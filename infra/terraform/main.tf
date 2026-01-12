# Networth Infrastructure - Main Configuration
# This Terraform configuration deploys the Azure infrastructure for the
# Networth personal finance application. The infrastructure includes:
# - Azure Container Apps for the Functions API
# - PostgreSQL Flexible Server for data storage
# - Azure Storage for queues and blobs
# - Azure Static Web Apps for the React frontend
# - Key Vault for secrets management
# - Application Insights for monitoring

resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}

locals {
  resource_suffix = "${var.environment}-${random_string.suffix.result}"

  # Queue names matching ResourceNames.cs in Networth.ServiceDefaults
  queue_names = {
    account_sync              = "account-sync"
    institution_sync          = "institution-sync"
    calculate_running_balance = "calculate-running-balance"
  }

  common_tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
    Repository  = "https://github.com/TomSB1423/NetWorth"
  }
}

# Resource Group

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.project_name}-${local.resource_suffix}"
  location = var.location
  tags     = local.common_tags
}
