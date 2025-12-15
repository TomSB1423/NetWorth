resource "azurerm_storage_account" "st" {
  name                     = "st${var.project_name}${random_string.suffix.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags                     = local.common_tags
}

resource "azurerm_storage_container" "blobs" {
  name                  = "blobs"
  storage_account_name  = azurerm_storage_account.st.name
  container_access_type = "private"
}

# Container for Function App deployment packages
resource "azurerm_storage_container" "deployment" {
  name                  = "deployment"
  storage_account_name  = azurerm_storage_account.st.name
  container_access_type = "private"
}

resource "azurerm_storage_queue" "account_sync" {
  name                 = "account-sync"
  storage_account_name = azurerm_storage_account.st.name
}

resource "azurerm_storage_queue" "institution_sync" {
  name                 = "institution-sync"
  storage_account_name = azurerm_storage_account.st.name
}

resource "azurerm_storage_queue" "calculate_running_balance" {
  name                 = "calculate-running-balance"
  storage_account_name = azurerm_storage_account.st.name
}

resource "azurerm_storage_table" "tables" {
  name                 = "funcstate"
  storage_account_name = azurerm_storage_account.st.name
}
