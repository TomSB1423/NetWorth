# Networth Terraform Deployment Plan

This document outlines the infrastructure-as-code strategy for deploying the Networth application to Azure using Terraform.

## Architecture Overview

The deployment replicates the .NET Aspire local development architecture using Azure-native PaaS services for cost-effectiveness and scalability.

### Infrastructure Components

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         Azure Resource Group                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐    ┌──────────────────┐    ┌──────────────────┐      │
│  │  Container App   │    │  Static Web App  │    │  PostgreSQL      │      │
│  │  (Functions API) │◄───│  (React SPA)     │    │  Flexible Server │      │
│  │                  │    │                  │    │                  │      │
│  └────────┬─────────┘    └──────────────────┘    └────────▲─────────┘      │
│           │                                               │                │
│           │              ┌──────────────────┐             │                │
│           └──────────────│  Storage Account │─────────────┘                │
│                          │  (Queues/Blobs)  │                              │
│                          └──────────────────┘                              │
│                                                                             │
│  ┌──────────────────┐    ┌──────────────────┐    ┌──────────────────┐      │
│  │  Container       │    │  Key Vault       │    │  App Insights    │      │
│  │  Registry (ACR)  │    │  (Secrets)       │    │  (Monitoring)    │      │
│  └──────────────────┘    └──────────────────┘    └──────────────────┘      │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                    Entra External ID (CIAM) Tenant                          │
│                    (Separate Terraform deployment)                          │
├─────────────────────────────────────────────────────────────────────────────┤
│  ┌──────────────────┐    ┌──────────────────┐                              │
│  │  API App Reg     │    │  SPA App Reg     │                              │
│  │  (networth-api)  │    │  (networth-spa)  │                              │
│  └──────────────────┘    └──────────────────┘                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Component Details

| Component | Azure Service | Purpose |
|-----------|---------------|---------|
| Functions API | Container App | Hosts the .NET 8 Azure Functions (HTTP + Queue triggers) |
| Frontend | Static Web App | Hosts the React SPA (Vite + TanStack Query) |
| Database | PostgreSQL Flexible Server | Stores users, accounts, transactions, balances |
| Queues | Storage Account Queues | Background job triggers (account-sync, etc.) |
| Secrets | Key Vault | Stores sensitive configuration |
| Monitoring | Application Insights | APM and logging |
| Container Registry | ACR | Docker images for the Functions app |
| Authentication | Entra External ID (CIAM) | Consumer-facing authentication |

## Terraform Structure

The configuration is split into two deployments:

### 1. Main Infrastructure (`infra/terraform/`)

| File | Purpose |
|------|---------|
| `providers.tf` | AzureRM and AzureAD provider configuration |
| `variables.tf` | Input variables including CIAM configuration |
| `main.tf` | Resource Group, locals, and random suffix |
| `container_apps.tf` | ACR, Container App Environment, Functions Container App |
| `postgres.tf` | PostgreSQL Flexible Server and database |
| `storage.tf` | Storage Account, queues (matching ResourceNames.cs) |
| `static_web_apps.tf` | Frontend React SPA hosting |
| `keyvault.tf` | Key Vault and secrets |
| `authentication.tf` | Data sources for Azure AD config |
| `outputs.tf` | URLs, resource names, configuration summary |

### 2. CIAM App Registrations (`infra/terraform-ciam/`)

Deployed separately to the Entra External ID tenant.

| File | Purpose |
|------|---------|
| `main.tf` | API and SPA app registrations |
| `variables.tf` | CIAM tenant configuration |
| `outputs.tf` | Client IDs and configuration values |

## Deployment Workflow

### Prerequisites

- Azure CLI (`az`)
- Terraform >= 1.5.0
- .NET 8 SDK
- Node.js & npm
- Docker (for building the Functions container)

### Step 1: Deploy CIAM App Registrations

First, deploy the app registrations to your CIAM tenant:

```bash
cd infra/terraform-ciam
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your CIAM tenant details

terraform init
terraform apply

# Note the outputs - you'll need these for Step 2
terraform output
```

### Step 2: Deploy Main Infrastructure

```bash
cd infra/terraform
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your values (including CIAM outputs from Step 1)

terraform init
terraform validate
terraform apply
```

### Step 3: Build and Push Docker Image

```bash
# Build the Functions container
docker build -t networth-functions -f Networth.Functions/Dockerfile .

# Tag and push to ACR
ACR_NAME=$(terraform -chdir=infra/terraform output -raw acr_name)
az acr login --name $ACR_NAME
docker tag networth-functions ${ACR_NAME}.azurecr.io/networth-functions:latest
docker push ${ACR_NAME}.azurecr.io/networth-functions:latest
```

### Step 4: Deploy Frontend

```bash
cd Networth.Frontend
npm install && npm run build

SWA_NAME=$(terraform -chdir=../infra/terraform output -raw frontend_swa_name)
RG_NAME=$(terraform -chdir=../infra/terraform output -raw resource_group_name)

az staticwebapp deploy -n $SWA_NAME -g $RG_NAME --source ./dist --env production
```

### Step 5: Update CIAM Redirect URIs

After deployment, update the CIAM SPA app registration with the production redirect URI:

```bash
# Get the frontend URL
FRONTEND_URL=$(terraform -chdir=infra/terraform output -raw frontend_swa_url)
echo "Add this redirect URI to your CIAM SPA app: ${FRONTEND_URL}/"
```

## Configuration Flow

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│ terraform-ciam/ │────▶│    terraform/   │────▶│   Application   │
│                 │     │                 │     │                 │
│ Outputs:        │     │ Uses as input:  │     │ Receives:       │
│ - api_client_id │     │ - ciam_*        │     │ - AzureAd__*    │
│ - spa_client_id │     │   variables     │     │ - VITE_ENTRA_*  │
│ - tenant_id     │     │                 │     │                 │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

## State Management

### Local State (Default)

- Stored in `terraform.tfstate` (gitignored)
- Good for solo development
- **Never commit to Git** (contains secrets)

### Remote State (Recommended for Teams)

Configure in `providers.tf`:

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "stterraformstate"
    container_name       = "tfstate"
    key                  = "networth.terraform.tfstate"
  }
}
```

## Security Notes

1. **PostgreSQL**: Only Azure services allowed by default (no "AllowAll" rule)
2. **Key Vault**: Stores all sensitive configuration with RBAC
3. **CIAM**: Consumer authentication isolated from main Azure tenant
4. **CORS**: Configured to only allow frontend and localhost origins
5. **TLS**: All public endpoints use HTTPS
