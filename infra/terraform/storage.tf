# Azure Storage Infrastructure
# This file creates the Azure Storage resources needed for:
# - Azure Functions host storage (AzureWebJobsStorage)
# - Queue storage for background job processing
# - Blob storage for deployment packages
#
# Queue names are aligned with ResourceNames.cs in Networth.ServiceDefaults

# Storage Account

resource "azurerm_storage_account" "st" {
  name                     = "st${var.project_name}${random_string.suffix.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  min_tls_version          = "TLS1_2"

  tags = local.common_tags
}

# Blob Containers

resource "azurerm_storage_container" "blobs" {
  name                  = "blobs"
  storage_account_name  = azurerm_storage_account.st.name
  container_access_type = "private"
}

resource "azurerm_storage_container" "deployment" {
  name                  = "deployment"
  storage_account_name  = azurerm_storage_account.st.name
  container_access_type = "private"
}

# Storage Queues
# Queue names match constants in Networth.ServiceDefaults.ResourceNames

# AccountSyncQueue - triggers account synchronization from GoCardless
resource "azurerm_storage_queue" "account_sync" {
  name                 = local.queue_names.account_sync
  storage_account_name = azurerm_storage_account.st.name
}

# InstitutionSyncQueue - triggers institution metadata sync
resource "azurerm_storage_queue" "institution_sync" {
  name                 = local.queue_names.institution_sync
  storage_account_name = azurerm_storage_account.st.name
}

# CalculateRunningBalanceQueue - triggers balance recalculation
resource "azurerm_storage_queue" "calculate_running_balance" {
  name                 = local.queue_names.calculate_running_balance
  storage_account_name = azurerm_storage_account.st.name
}

# Storage Table (for Functions state)

resource "azurerm_storage_table" "funcstate" {
  name                 = "funcstate"
  storage_account_name = azurerm_storage_account.st.name
}
