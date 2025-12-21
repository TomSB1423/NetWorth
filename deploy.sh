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

# Configuration
MAIN_SUBSCRIPTION="420d3eea-49f8-4937-8a81-b7aac3471b55"
CIAM_TENANT_ID="e2a8021c-d32c-470d-8703-af48ca780354"
CIAM_SPA_CLIENT_ID="5709e63b-afea-4e48-86f8-1f2f46aabffd"

# Parse command line arguments
SKIP_INFRA=false
SKIP_BACKEND=false
SKIP_FRONTEND=false
SKIP_CIAM_UPDATE=false

usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --skip-infra      Skip Terraform infrastructure provisioning"
    echo "  --skip-backend    Skip backend Docker build and deployment"
    echo "  --skip-frontend   Skip frontend build and deployment"
    echo "  --skip-ciam       Skip CIAM redirect URI update"
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
        --skip-ciam)
            SKIP_CIAM_UPDATE=true
            shift
            ;;
        --backend-only)
            SKIP_INFRA=true
            SKIP_FRONTEND=true
            SKIP_CIAM_UPDATE=true
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
cd "$SCRIPT_DIR"

echo -e "${GREEN}Configuration:${NC}"
echo "  Resource Group: $RG_NAME"
echo "  Container App: $ACA_NAME"
echo "  ACR: $ACR_LOGIN_SERVER"
echo "  Frontend SWA: $FRONTEND_SWA_NAME"
echo "  Frontend URL: $FRONTEND_URL"

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
    cd "$SCRIPT_DIR/Networth.Frontend"
    
    echo "Installing dependencies..."
    npm install --silent
    
    echo "Building frontend..."
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

# 4. Update CIAM Redirect URIs
if [ "$SKIP_CIAM_UPDATE" = false ]; then
    echo -e "${BLUE}Step 4: Updating CIAM Redirect URIs...${NC}"
    
    # Get current redirect URIs
    CURRENT_URIS=$(az ad app show --id "$CIAM_SPA_CLIENT_ID" --query "spa.redirectUris" -o json 2>/dev/null || echo "[]")
    
    # Check if frontend URL is already in redirect URIs
    FRONTEND_URI="${FRONTEND_URL}/"
    if echo "$CURRENT_URIS" | grep -q "$FRONTEND_URI"; then
        echo -e "${GREEN}CIAM redirect URIs already configured correctly${NC}"
    else
        echo "Adding $FRONTEND_URI to CIAM redirect URIs..."
        
        # Switch to CIAM tenant temporarily
        az login --tenant "$CIAM_TENANT_ID" --allow-no-subscriptions --only-show-errors >/dev/null 2>&1 || true
        
        # Get the app object ID
        APP_OBJECT_ID=$(az ad app show --id "$CIAM_SPA_CLIENT_ID" --query id -o tsv)
        
        # Update redirect URIs via Graph API
        az rest --method PATCH \
          --uri "https://graph.microsoft.com/v1.0/applications/$APP_OBJECT_ID" \
          --headers "Content-Type=application/json" \
          --body "{\"spa\":{\"redirectUris\":[\"http://localhost:3000/\",\"http://localhost:5173/\",\"$FRONTEND_URI\"]}}" \
          --output none
        
        echo -e "${GREEN}CIAM redirect URIs updated!${NC}"
        
        # Switch back to main subscription
        az account set --subscription "$MAIN_SUBSCRIPTION"
    fi
else
    echo -e "${YELLOW}Skipping CIAM redirect URI update...${NC}"
fi

echo ""
echo -e "${GREEN}╔═══════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║              DEPLOYMENT COMPLETE!                              ║${NC}"
echo -e "${GREEN}╠═══════════════════════════════════════════════════════════════╣${NC}"
echo -e "${GREEN}║${NC} Frontend: ${BLUE}$FRONTEND_URL${NC}"
echo -e "${GREEN}║${NC} API:      ${BLUE}https://$ACA_NAME.*.azurecontainerapps.io${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════════════════╝${NC}"
