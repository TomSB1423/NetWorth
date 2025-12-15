resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}

locals {
  resource_suffix = "${var.environment}-${random_string.suffix.result}"
  common_tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.project_name}-${local.resource_suffix}"
  location = var.location
  tags     = local.common_tags
}
