# Networth Terraform Deployment Plan

This document outlines the infrastructure-as-code strategy for deploying the Networth application to Azure using Terraform.

## Architecture Overview

The deployment will replicate the .NET Aspire architecture using Azure-native PaaS services for cost-effectiveness and scalability.

### Components

1. **Resource Group**: A logical container for all resources.
2. **Networking**:
    - Basic VNet integration can be added for enhanced security (Postgres/Functions), but for the initial setup, we will use public access with restricted firewall rules for simplicity, mirroring the development environment.
3. **Database**:
    - **Azure Database for PostgreSQL Flexible Server**:
        - SKU: Burstable (B1ms) for cost efficiency in dev/test.
        - Database: `networth-db`.
        - Auth: PostgreSQL authentication (username/password).
4. **Storage**:
    - **Azure Storage Account** (Standard LRS):
        - **Queues**: `account-sync`, `institution-sync`, `calculate-running-balance`.
        - **Blobs**: `blobs` container (for WebJobs/Functions lease).
        - **Tables**: `tables` (for WebJobs/Functions).
5. **Compute - Backend**:
    - **Azure Function App** (Linux):
        - Plan: Consumption (Y1) or Premium (EP1). We will use Consumption for cost savings.
        - Runtime: .NET 8 Isolated.
        - Application Insights: For monitoring and logging.
        - Configuration: Connection strings for Postgres and Storage, GoCardless secrets.
6. **Compute - Frontend & Docs**:
    - **Azure Static Web Apps**:
        - **Frontend**: Hosts the React SPA.
        - **Docs**: Hosts the Docusaurus site.
        - Tier: Free or Standard.

## Terraform Structure

We will organize the Terraform configuration into modular files:

- `providers.tf`: AzureRM provider configuration.
- `variables.tf`: Input variables (location, secrets, naming prefixes).
- `main.tf`: Resource Group and shared resources (Random ID).
- `postgres.tf`: Database server and firewall rules.
- `storage.tf`: Storage account and queues.
- `functions.tf`: Function App, Service Plan, App Insights.
- `static_web_apps.tf`: Frontend and Docs resources.
- `outputs.tf`: Important URLs and connection strings.

## Deployment Workflow

1. **Infrastructure Provisioning**:

    ```bash
    cd infra/terraform
    terraform init
    terraform apply
    ```

2. **Application Deployment**:
    - **Functions**:

        ```bash
        dotnet publish Networth.Functions/Networth.Functions.csproj -c Release -o ./publish
        cd ./publish && zip -r ../functions.zip .
        az functionapp deployment source config-zip -g <resource-group> -n <function-app-name> --src functions.zip
        ```

    - **Frontend**:

        ```bash
        cd Networth.Frontend
        npm install && npm run build
        az staticwebapp deploy --name <frontend-swa-name> --resource-group <resource-group> --source ./dist --env production
        ```

    - **Docs**:

        ```bash
        cd Networth.Docs
        npm install && npm run build
        az staticwebapp deploy --name <docs-swa-name> --resource-group <resource-group> --source ./build --env production
        ```

## Prerequisites

- Azure CLI (`az`)
- Terraform
- .NET 8 SDK
- Node.js & npm
- Azure Static Web Apps CLI (`swa`) (optional, can use `az staticwebapp` commands)

## Development Cycle & Concepts

### 1. Infrastructure vs. Application Code

It is important to understand that **Terraform only manages infrastructure** (the "empty shell" of your resources). It does **not** build or deploy your application code.

- **Terraform**: Creates the Resource Group, Function App resource, Postgres Server, Storage Accounts, etc.
- **Manual/CI Steps**: Compiles your C#/.NET code, builds your React app, and uploads the artifacts to the resources created by Terraform.

**The Cycle:**

1. **Modify Infrastructure**: Change `.tf` files -> `terraform apply`.
2. **Modify App Code**: Change `.cs` or `.tsx` files -> Build -> Deploy via `az` CLI.

### 2. State Management

Terraform tracks the state of your resources (IDs, current configuration) in a **State File**.

- **Local State (Default)**:
  - Stored in `infra/terraform/terraform.tfstate`.
  - **Pros**: Simple, zero config. Good for solo development.
  - **Cons**: If you delete this file, Terraform loses track of your resources and will try to create them again (causing errors). **Do not commit this file to Git** (it may contain secrets).

- **Remote State (Recommended for Teams/CI)**:
  - Stored in an Azure Storage Account container.
  - Allows multiple developers and CI pipelines to share the same view of the infrastructure.
  - Requires configuring a `backend "azurerm"` block in `providers.tf`.

### 3. Detailed CLI Deployment Steps

When working locally, follow this sequence:

#### Step A: Infrastructure (Terraform)

1. **Init**: Downloads providers. Run once per project.

    ```bash
    terraform init
    ```

2. **Plan**: Preview changes.

    ```bash
    terraform plan
    ```

3. **Apply**: Create/Update resources.

    ```bash
    terraform apply
    ```

    *Note the outputs (Resource Group Name, App Names) for Step B.*

#### Step B: Application (Azure CLI)

**Backend (.NET Functions):**
The Function App resource exists, but it's empty.

```bash
# 1. Build
dotnet publish Networth.Functions/Networth.Functions.csproj -c Release -o ./publish
# 2. Zip
cd publish && zip -r ../functions.zip . && cd ..
# 3. Deploy
az functionapp deployment source config-zip -g <rg-name> -n <func-name> --src functions.zip
```

**Frontend (React):**
The Static Web App resource exists, but has no content.

```bash
# 1. Build
cd Networth.Frontend
npm install && npm run build
# 2. Deploy
az staticwebapp deploy -n <swa-name> -g <rg-name> --source ./dist --env production
```
