# =============================================================================
# Azure Static Web Apps
# =============================================================================
# This file creates the Azure Static Web App for hosting the React frontend.
# The frontend is built with Vite + React + TanStack Query + Tailwind CSS.
#
# Note: Static Web Apps require deployment via GitHub Actions or Azure DevOps.
# The Terraform resource creates the hosting infrastructure only.
# =============================================================================

resource "azurerm_static_web_app" "frontend" {
  name                = "stapp-frontend-${var.project_name}-${local.resource_suffix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = "eastus2" # SWA is a global service, but needs a resource location

  sku_tier = "Free"
  sku_size = "Free"

  tags = local.common_tags
}

# -----------------------------------------------------------------------------
# Notes on Frontend Configuration
# -----------------------------------------------------------------------------
# The frontend requires the following environment variables at build time:
#
# VITE_API_URL            = <container_app_url>
# VITE_ENTRA_CLIENT_ID    = <ciam_spa_client_id>
# VITE_ENTRA_TENANT_ID    = <ciam_tenant_id>
# VITE_ENTRA_INSTANCE     = https://<ciam_tenant_domain>.ciamlogin.com/
# VITE_ENTRA_API_CLIENT_ID = <ciam_api_client_id>
# VITE_ENTRA_SCOPES       = <ciam_api_client_id>/.default
#
# These should be configured in the GitHub Actions workflow or via the
# Azure Portal Static Web Apps configuration.

