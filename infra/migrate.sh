#!/bin/bash
set -e

# Database Migration Script for Networth
# This script applies EF Core migrations to the database
# Usage: ./migrate.sh [connection-string]
#
# If no connection string is provided, it will use Terraform outputs to construct one.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== Networth Database Migration ===${NC}"

# Check if connection string is provided as argument
if [ -n "$1" ]; then
    CONNECTION_STRING="$1"
    echo -e "${BLUE}Using provided connection string${NC}"
else
    echo -e "${BLUE}Building connection string from Terraform outputs...${NC}"

    # Get values from Terraform
    cd "$ROOT_DIR/infra/terraform"

    POSTGRES_FQDN=$(terraform output -raw postgres_fqdn 2>/dev/null || echo "")

    if [ -z "$POSTGRES_FQDN" ]; then
        echo -e "${RED}Error: Could not get postgres_fqdn from Terraform outputs${NC}"
        echo -e "${YELLOW}Make sure Terraform has been applied and you're authenticated to Azure${NC}"
        exit 1
    fi

    # Get password from Key Vault
    KV_NAME=$(terraform output -raw key_vault_name 2>/dev/null || echo "")

    if [ -z "$KV_NAME" ]; then
        echo -e "${RED}Error: Could not get key_vault_name from Terraform outputs${NC}"
        exit 1
    fi

    echo -e "${BLUE}Fetching database password from Key Vault: $KV_NAME${NC}"
    POSTGRES_PASSWORD=$(az keyvault secret show --name postgres-admin-password --vault-name "$KV_NAME" --query value -o tsv 2>/dev/null || echo "")

    if [ -z "$POSTGRES_PASSWORD" ]; then
        echo -e "${RED}Error: Could not retrieve postgres password from Key Vault${NC}"
        echo -e "${YELLOW}Make sure you have access to the Key Vault${NC}"
        exit 1
    fi

    CONNECTION_STRING="Host=${POSTGRES_FQDN};Database=networth-db;Username=networthadmin;Password=${POSTGRES_PASSWORD};SSL Mode=Require;Trust Server Certificate=true"

    cd "$ROOT_DIR"
fi

echo -e "${BLUE}Applying migrations...${NC}"

# Run EF Core migrations
cd "$ROOT_DIR/Networth.Infrastructure"

# Apply migrations using dotnet ef with connection string passed directly
dotnet ef database update \
    --project "$ROOT_DIR/Networth.Infrastructure/Networth.Infrastructure.csproj" \
    --connection "$CONNECTION_STRING" \
    --verbose

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Migrations applied successfully!${NC}"
else
    echo -e "${RED}✗ Migration failed${NC}"
    exit 1
fi
