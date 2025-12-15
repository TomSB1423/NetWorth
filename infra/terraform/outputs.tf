output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "container_app_name" {
  value = azurerm_container_app.functions.name
}

output "container_app_url" {
  value = "https://${azurerm_container_app.functions.ingress[0].fqdn}"
}

output "acr_login_server" {
  value = azurerm_container_registry.acr.login_server
}

output "acr_name" {
  value = azurerm_container_registry.acr.name
}

output "frontend_swa_name" {
  value = azurerm_static_web_app.frontend.name
}

output "frontend_swa_url" {
  value = azurerm_static_web_app.frontend.default_host_name
}

output "postgres_server_name" {
  value = azurerm_postgresql_flexible_server.psql.name
}

output "postgres_fqdn" {
  value = azurerm_postgresql_flexible_server.psql.fqdn
}

output "key_vault_name" {
  value = azurerm_key_vault.kv.name
}

output "key_vault_uri" {
  value = azurerm_key_vault.kv.vault_uri
}

output "entra_api_client_id" {
  value       = azuread_application.api.client_id
  description = "API App Registration Client ID for authentication"
}

output "entra_frontend_client_id" {
  value       = azuread_application.frontend.client_id
  description = "Frontend SPA App Registration Client ID"
}

output "entra_tenant_id" {
  value       = data.azuread_client_config.current.tenant_id
  description = "Microsoft Entra Tenant ID"
}

