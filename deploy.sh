#!/bin/bash
set -e # Exit on error

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}Starting Networth Deployment...${NC}"

# 1. Provision Infrastructure with Terraform
echo -e "${BLUE}Step 1: Provisioning Infrastructure...${NC}"
cd infra/terraform
terraform init
terraform apply -auto-approve

# Capture Outputs
RG_NAME=$(terraform output -raw resource_group_name)
ACA_NAME=$(terraform output -raw container_app_name)
ACR_NAME=$(terraform output -raw acr_name)
ACR_LOGIN_SERVER=$(terraform output -raw acr_login_server)
FRONTEND_SWA_NAME=$(terraform output -raw frontend_swa_name)

echo -e "${GREEN}Infrastructure Ready!${NC}"
echo "Resource Group: $RG_NAME"
echo "Container App: $ACA_NAME"
echo "ACR: $ACR_LOGIN_SERVER"
echo "Frontend SWA: $FRONTEND_SWA_NAME"

# Return to root
cd ../..

# 2. Deploy Backend (Container App)
echo -e "${BLUE}Step 2: Deploying Backend (Container App)...${NC}"

# Login to ACR
echo "Logging into ACR..."
az acr login --name "$ACR_NAME"

# Build and Push Docker Image
IMAGE_TAG="latest"
IMAGE_NAME="$ACR_LOGIN_SERVER/networth-functions:$IMAGE_TAG"

echo "Building Docker image..."
# Build from root context
docker build -t "$IMAGE_NAME" -f Networth.Functions/Dockerfile .

echo "Pushing Docker image..."
docker push "$IMAGE_NAME"

echo "Updating Container App..."
az containerapp update \
  --name "$ACA_NAME" \
  --resource-group "$RG_NAME" \
  --image "$IMAGE_NAME"

# 3. Deploy Frontend
echo -e "${BLUE}Step 3: Deploying Frontend...${NC}"
cd Networth.Frontend
echo "Installing dependencies and building..."
npm install
npm run build

echo "Deploying to Static Web App..."
DEPLOYMENT_TOKEN=$(az staticwebapp secrets list --name "$FRONTEND_SWA_NAME" --resource-group "$RG_NAME" --query "properties.apiKey" -o tsv)

swa deploy ./dist \
  --env production \
  --deployment-token "$DEPLOYMENT_TOKEN"

cd ..

echo -e "${GREEN}Deployment Complete!${NC}"
