#!/bin/bash
set -e # Exit on error

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Configuration - set via environment variable or override here
MAIN_SUBSCRIPTION="${AZURE_SUBSCRIPTION_ID:?Error: AZURE_SUBSCRIPTION_ID environment variable is required}"

# Parse command line arguments
SKIP_INFRA=false
SKIP_BACKEND=false
SKIP_FRONTEND=false

usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --skip-infra      Skip Terraform infrastructure provisioning"
    echo "  --skip-backend    Skip backend Docker build and deployment"
    echo "  --skip-frontend   Skip frontend build and deployment"
    echo "  --backend-only    Only deploy backend"
    echo "  --frontend-only   Only deploy frontend"
    echo "  -h, --help        Show this help message"
    exit 0
}

while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-infra)
            SKIP_INFRA=true
            shift
            ;;
        --skip-backend)
            SKIP_BACKEND=true
            shift
            ;;
        --skip-frontend)
            SKIP_FRONTEND=true
            shift
            ;;
        --backend-only)
            SKIP_INFRA=true
            SKIP_FRONTEND=true
            shift
            ;;
        --frontend-only)
            SKIP_INFRA=true
            SKIP_BACKEND=true
            shift
            ;;
        -h|--help)
            usage
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            usage
            ;;
    esac
done

echo -e "${BLUE}Starting Networth Deployment...${NC}"

# Ensure we're logged into the correct Azure subscription
echo -e "${BLUE}Checking Azure subscription...${NC}"
CURRENT_SUB=$(az account show --query id -o tsv 2>/dev/null || echo "")
if [ "$CURRENT_SUB" != "$MAIN_SUBSCRIPTION" ]; then
    echo -e "${YELLOW}Switching to main Azure subscription...${NC}"
    az account set --subscription "$MAIN_SUBSCRIPTION"
fi

# 1. Provision Infrastructure with Terraform
if [ "$SKIP_INFRA" = false ]; then
    echo -e "${BLUE}Step 1: Provisioning Infrastructure...${NC}"
    cd "$SCRIPT_DIR/infra/terraform"
    terraform init -upgrade
    terraform apply -auto-approve
    cd "$SCRIPT_DIR"
else
    echo -e "${YELLOW}Skipping infrastructure provisioning...${NC}"
fi

# Capture Terraform Outputs (always needed for other steps)
cd "$SCRIPT_DIR/infra/terraform"
RG_NAME=$(terraform output -raw resource_group_name)
ACA_NAME=$(terraform output -raw container_app_name)
ACR_NAME=$(terraform output -raw acr_name)
ACR_LOGIN_SERVER=$(terraform output -raw acr_login_server)
FRONTEND_SWA_NAME=$(terraform output -raw frontend_swa_name)
FRONTEND_URL=$(terraform output -raw frontend_swa_url)
API_URL=$(terraform output -raw container_app_url)
cd "$SCRIPT_DIR"

echo -e "${GREEN}Configuration:${NC}"
echo "  Resource Group: $RG_NAME"
echo "  Container App: $ACA_NAME"
echo "  ACR: $ACR_LOGIN_SERVER"
echo "  Frontend SWA: $FRONTEND_SWA_NAME"
echo "  Frontend URL: $FRONTEND_URL"
echo "  API URL: $API_URL"

# 2. Deploy Backend (Container App)
if [ "$SKIP_BACKEND" = false ]; then
    echo -e "${BLUE}Step 2: Deploying Backend (Container App)...${NC}"

    # Login to ACR
    echo "Logging into ACR..."
    az acr login --name "$ACR_NAME"

    # Build and Push Docker Image with timestamp tag for cache busting
    TIMESTAMP=$(date +%Y%m%d%H%M%S)
    IMAGE_TAG_LATEST="latest"
    IMAGE_TAG_VERSIONED="v$TIMESTAMP"
    IMAGE_NAME_LATEST="$ACR_LOGIN_SERVER/networth-functions:$IMAGE_TAG_LATEST"
    IMAGE_NAME_VERSIONED="$ACR_LOGIN_SERVER/networth-functions:$IMAGE_TAG_VERSIONED"

    echo "Building Docker image..."
    docker build -t "$IMAGE_NAME_LATEST" -t "$IMAGE_NAME_VERSIONED" -f Networth.Functions/Dockerfile .

    echo "Pushing Docker images..."
    docker push "$IMAGE_NAME_LATEST"
    docker push "$IMAGE_NAME_VERSIONED"

    echo "Updating Container App..."
    az containerapp update \
      --name "$ACA_NAME" \
      --resource-group "$RG_NAME" \
      --image "$IMAGE_NAME_LATEST" \
      --output none

    echo -e "${GREEN}Backend deployed!${NC}"
else
    echo -e "${YELLOW}Skipping backend deployment...${NC}"
fi

# 3. Deploy Frontend
if [ "$SKIP_FRONTEND" = false ]; then
    echo -e "${BLUE}Step 3: Deploying Frontend...${NC}"

    # Get Firebase config from Terraform (required for production builds)
    cd "$SCRIPT_DIR/infra/terraform"
    FIREBASE_API_KEY=$(terraform output -raw firebase_api_key)
    FIREBASE_AUTH_DOMAIN=$(terraform output -raw firebase_auth_domain)
    FIREBASE_PROJECT_ID=$(terraform output -raw firebase_project_id)
    FIREBASE_STORAGE_BUCKET=$(terraform output -raw firebase_storage_bucket)
    FIREBASE_MESSAGING_SENDER_ID=$(terraform output -raw firebase_messaging_sender_id)
    FIREBASE_APP_ID=$(terraform output -raw firebase_app_id)

    # Validate required config
    if [ -z "$FIREBASE_API_KEY" ] || [ -z "$FIREBASE_PROJECT_ID" ]; then
        echo -e "${RED}Error: Firebase configuration is missing. Ensure terraform has been applied with Firebase variables.${NC}"
        exit 1
    fi

    if [ -z "$API_URL" ]; then
        echo -e "${RED}Error: API_URL is missing. Ensure terraform outputs container_app_url.${NC}"
        exit 1
    fi

    cd "$SCRIPT_DIR/Networth.Frontend"

    echo "Installing dependencies..."
    npm install --silent

    echo "Building frontend with API_URL=$API_URL..."
    VITE_API_URL="$API_URL" \
    VITE_USE_MOCK_DATA="false" \
    VITE_FIREBASE_API_KEY="$FIREBASE_API_KEY" \
    VITE_FIREBASE_AUTH_DOMAIN="$FIREBASE_AUTH_DOMAIN" \
    VITE_FIREBASE_PROJECT_ID="$FIREBASE_PROJECT_ID" \
    VITE_FIREBASE_STORAGE_BUCKET="$FIREBASE_STORAGE_BUCKET" \
    VITE_FIREBASE_MESSAGING_SENDER_ID="$FIREBASE_MESSAGING_SENDER_ID" \
    VITE_FIREBASE_APP_ID="$FIREBASE_APP_ID" \
    npm run build

    echo "Deploying to Static Web App..."
    DEPLOYMENT_TOKEN=$(az staticwebapp secrets list --name "$FRONTEND_SWA_NAME" --resource-group "$RG_NAME" --query "properties.apiKey" -o tsv)

    swa deploy ./dist \
      --env production \
      --deployment-token "$DEPLOYMENT_TOKEN"

    cd "$SCRIPT_DIR"
    echo -e "${GREEN}Frontend deployed!${NC}"
else
    echo -e "${YELLOW}Skipping frontend deployment...${NC}"
fi

echo ""
echo -e "${GREEN}Deployment complete!${NC}"
echo "  Frontend: $FRONTEND_URL"
echo "  API:      $API_URL"
