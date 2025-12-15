resource "azurerm_static_web_app" "frontend" {
  name                = "stapp-frontend-${var.project_name}-${local.resource_suffix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = "eastus2" # Static Web Apps are global but resource needs a location. East US 2 is standard.
  sku_tier            = "Free"
  sku_size            = "Free"

  tags = local.common_tags
}

