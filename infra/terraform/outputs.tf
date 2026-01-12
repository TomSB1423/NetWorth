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

output "firebase_api_key" {
  value       = var.firebase_api_key
  description = "Firebase API Key (for frontend)"
  sensitive   = true
}

output "firebase_storage_bucket" {
  value       = var.firebase_storage_bucket
  description = "Firebase Storage Bucket (for frontend)"
}

output "firebase_messaging_sender_id" {
  value       = var.firebase_messaging_sender_id
  description = "Firebase Messaging Sender ID (for frontend)"
}

output "firebase_app_id" {
  value       = var.firebase_app_id
  description = "Firebase App ID (for frontend)"
}

