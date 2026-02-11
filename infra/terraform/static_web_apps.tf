# Frontend Static Web App (Vite + React + TanStack Query + Tailwind)
# Deploy via GitHub Actions or Azure DevOps.

resource "azurerm_static_web_app" "frontend" {
  name                = "stapp-frontend-${var.project_name}-${local.resource_suffix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = "eastus2" # SWA is a global service, but needs a resource location

  sku_tier = "Free"
  sku_size = "Free"

  tags = local.common_tags
}

# Frontend environment variables (set in GitHub Actions or Azure Portal):
#
#   VITE_API_URL                      = <container_app_url>
#   VITE_FIREBASE_API_KEY             = <firebase_api_key>
#   VITE_FIREBASE_AUTH_DOMAIN         = <firebase_auth_domain>
#   VITE_FIREBASE_PROJECT_ID          = <firebase_project_id>
#   VITE_FIREBASE_STORAGE_BUCKET      = <firebase_storage_bucket>
#   VITE_FIREBASE_MESSAGING_SENDER_ID = <firebase_messaging_sender_id>
#   VITE_FIREBASE_APP_ID              = <firebase_app_id>
#
# Get firebase_api_key from Key Vault:
#   az keyvault secret show --vault-name <kv_name> --name firebase-api-key

