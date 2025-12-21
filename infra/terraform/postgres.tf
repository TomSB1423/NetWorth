# =============================================================================
# PostgreSQL Flexible Server
# =============================================================================
# This file creates the PostgreSQL Flexible Server for storing:
# - User accounts and preferences
# - Bank account data (accounts, transactions, balances)
# - GoCardless requisitions and agreements
#
# Database name matches ResourceNames.NetworthDb in Networth.ServiceDefaults
# =============================================================================

# -----------------------------------------------------------------------------
# PostgreSQL Flexible Server
# -----------------------------------------------------------------------------

resource "azurerm_postgresql_flexible_server" "psql" {
  name                   = "psql-${var.project_name}-${local.resource_suffix}"
  resource_group_name    = azurerm_resource_group.rg.name
  location               = azurerm_resource_group.rg.location
  version                = "16"
  administrator_login    = "networthadmin"
  administrator_password = var.postgres_admin_password
  storage_mb             = 32768
  sku_name               = "B_Standard_B1ms"
  zone                   = "1"

  tags = local.common_tags
}

# -----------------------------------------------------------------------------
# Database
# -----------------------------------------------------------------------------

resource "azurerm_postgresql_flexible_server_database" "db" {
  name      = "networth-db"
  server_id = azurerm_postgresql_flexible_server.psql.id
  collation = "en_US.utf8"
  charset   = "utf8"
}

# -----------------------------------------------------------------------------
# Firewall Rules
# -----------------------------------------------------------------------------
# Allow Azure services to connect (required for Container Apps)

resource "azurerm_postgresql_flexible_server_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_postgresql_flexible_server.psql.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# Note: For production, consider using:
# - Private endpoints instead of public access
# - VNet integration with the Container App Environment
# - More restrictive firewall rules
