# =============================================================================
# Outputs
# =============================================================================
# These outputs provide important values needed for:
# - CI/CD deployment pipelines
# - Local development configuration
# - Application configuration
# =============================================================================

# -----------------------------------------------------------------------------
# Resource Group
# -----------------------------------------------------------------------------

output "resource_group_name" {
  value       = azurerm_resource_group.rg.name
  description = "The name of the resource group"
}

# -----------------------------------------------------------------------------
# Container App
# -----------------------------------------------------------------------------

output "container_app_name" {
  value       = azurerm_container_app.functions.name
  description = "The name of the Container App"
}

output "container_app_url" {
  value       = "https://${azurerm_container_app.functions.ingress[0].fqdn}"
  description = "The public URL of the Container App (API)"
}

# -----------------------------------------------------------------------------
# Container Registry
# -----------------------------------------------------------------------------

output "acr_login_server" {
  value       = azurerm_container_registry.acr.login_server
  description = "The login server URL for ACR"
}

output "acr_name" {
  value       = azurerm_container_registry.acr.name
  description = "The name of the Azure Container Registry"
}

# -----------------------------------------------------------------------------
# Frontend
# -----------------------------------------------------------------------------

output "frontend_swa_name" {
  value       = azurerm_static_web_app.frontend.name
  description = "The name of the Static Web App"
}

output "frontend_swa_url" {
  value       = "https://${azurerm_static_web_app.frontend.default_host_name}"
  description = "The public URL of the frontend Static Web App"
}

# -----------------------------------------------------------------------------
# Database
# -----------------------------------------------------------------------------

output "postgres_server_name" {
  value       = azurerm_postgresql_flexible_server.psql.name
  description = "The name of the PostgreSQL Flexible Server"
}

output "postgres_fqdn" {
  value       = azurerm_postgresql_flexible_server.psql.fqdn
  description = "The FQDN of the PostgreSQL server"
}

# -----------------------------------------------------------------------------
# Key Vault
# -----------------------------------------------------------------------------

output "key_vault_name" {
  value       = azurerm_key_vault.kv.name
  description = "The name of the Key Vault"
}

output "key_vault_uri" {
  value       = azurerm_key_vault.kv.vault_uri
  description = "The URI of the Key Vault"
}

# -----------------------------------------------------------------------------
# CIAM / Authentication (values passed through from variables)
# -----------------------------------------------------------------------------

output "ciam_authority" {
  value       = "https://${var.ciam_tenant_domain}.ciamlogin.com/${var.ciam_tenant_id}"
  description = "MSAL Authority URL for CIAM"
}

output "ciam_api_client_id" {
  value       = var.ciam_api_client_id
  description = "API Application (Client) ID from CIAM"
}

output "ciam_spa_client_id" {
  value       = var.ciam_spa_client_id
  description = "SPA Application (Client) ID from CIAM"
}

output "ciam_tenant_id" {
  value       = var.ciam_tenant_id
  description = "CIAM Tenant ID"
}

# -----------------------------------------------------------------------------
# Configuration Summary
# -----------------------------------------------------------------------------

output "configuration_summary" {
  value       = <<-EOT

    ╔═══════════════════════════════════════════════════════════════════════════╗
    ║                   DEPLOYMENT CONFIGURATION SUMMARY                        ║
    ╠═══════════════════════════════════════════════════════════════════════════╣
    ║ API Container App:                                                        ║
    ║   URL: https://${azurerm_container_app.functions.ingress[0].fqdn}
    ║   ACR: ${azurerm_container_registry.acr.login_server}
    ╠═══════════════════════════════════════════════════════════════════════════╣
    ║ Frontend Static Web App:                                                  ║
    ║   URL: https://${azurerm_static_web_app.frontend.default_host_name}
    ╠═══════════════════════════════════════════════════════════════════════════╣
    ║ Database:                                                                 ║
    ║   PostgreSQL: ${azurerm_postgresql_flexible_server.psql.fqdn}
    ╠═══════════════════════════════════════════════════════════════════════════╣
    ║ CIAM Authentication:                                                      ║
    ║   Authority: https://${var.ciam_tenant_domain}.ciamlogin.com/
    ║   API Client: ${var.ciam_api_client_id}
    ║   SPA Client: ${var.ciam_spa_client_id}
    ╚═══════════════════════════════════════════════════════════════════════════╝

    NEXT STEPS:
    1. Update CIAM redirect URIs with the frontend URL
    2. Push Docker image to ACR: ${azurerm_container_registry.acr.login_server}/networth-functions:latest
    3. Deploy frontend to Static Web App

  EOT
  description = "Summary of deployed resources and next steps"
}

