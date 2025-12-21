# Resource Group

output "resource_group_name" {
  value       = azurerm_resource_group.rg.name
  description = "The name of the resource group"
}

# Container App

output "container_app_name" {
  value       = azurerm_container_app.functions.name
  description = "The name of the Container App"
}

output "container_app_url" {
  value       = "https://${azurerm_container_app.functions.ingress[0].fqdn}"
  description = "The public URL of the Container App (API)"
}

# Container Registry

output "acr_login_server" {
  value       = azurerm_container_registry.acr.login_server
  description = "The login server URL for ACR"
}

output "acr_name" {
  value       = azurerm_container_registry.acr.name
  description = "The name of the Azure Container Registry"
}

# Frontend

output "frontend_swa_name" {
  value       = azurerm_static_web_app.frontend.name
  description = "The name of the Static Web App"
}

output "frontend_swa_url" {
  value       = "https://${azurerm_static_web_app.frontend.default_host_name}"
  description = "The public URL of the frontend Static Web App"
}

# Database

output "postgres_server_name" {
  value       = azurerm_postgresql_flexible_server.psql.name
  description = "The name of the PostgreSQL Flexible Server"
}

output "postgres_fqdn" {
  value       = azurerm_postgresql_flexible_server.psql.fqdn
  description = "The FQDN of the PostgreSQL server"
}

# Key Vault

output "key_vault_name" {
  value       = azurerm_key_vault.kv.name
  description = "The name of the Key Vault"
}

output "key_vault_uri" {
  value       = azurerm_key_vault.kv.vault_uri
  description = "The URI of the Key Vault"
}

# Firebase

output "firebase_project_id" {
  value       = var.firebase_project_id
  description = "Firebase Project ID"
}

output "firebase_auth_domain" {
  value       = var.firebase_auth_domain
  description = "Firebase Auth Domain (for frontend)"
}

# Sensitive values stored in Key Vault:
#   az keyvault secret show --vault-name <key_vault_name> --name <secret-name>

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
    ║ Firebase Authentication:                                                  ║
    ║   Project: ${var.firebase_project_id}
    ║   Auth Domain: ${var.firebase_auth_domain}
    ║   API Key: Stored in Key Vault (firebase-api-key)
    ║   Service Account: Stored in Key Vault (firebase-service-account)
    ╚═══════════════════════════════════════════════════════════════════════════╝

    NEXT STEPS:
    1. Configure Firebase authorized domains with the frontend URL
    2. Push Docker image to ACR: ${azurerm_container_registry.acr.login_server}/networth-functions:latest
    3. Deploy frontend to Static Web App with Firebase env vars

  EOT
  description = "Summary of deployed resources and next steps"
}

