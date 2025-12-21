# =============================================================================
# CIAM Identity Outputs
# =============================================================================
# These values are needed to configure the frontend and backend applications.
# They should be stored as secrets/environment variables.
#
# NOTE: In CIAM, we use the API's client_id as both the audience and scope
# since custom API scopes are not supported.
# =============================================================================

output "spa_client_id" {
  value       = azuread_application.spa.client_id
  description = "SPA Application (Client) ID - use for VITE_ENTRA_CLIENT_ID"
}

output "api_client_id" {
  value       = azuread_application.api.client_id
  description = "API Application (Client) ID - use for AzureAd:ClientId and AzureAd:Audience"
}

output "api_scope" {
  value       = "api://${var.project_name}-api/user_impersonation"
  description = "Scope to request for API access - use for VITE_ENTRA_SCOPES"
}

output "tenant_id" {
  value       = var.ciam_tenant_id
  description = "CIAM Tenant ID"
}

output "authority" {
  value       = "https://${var.ciam_tenant_domain}.ciamlogin.com/${var.ciam_tenant_id}"
  description = "MSAL Authority URL - use for VITE_ENTRA_AUTHORITY"
}

output "instance" {
  value       = "https://${var.ciam_tenant_domain}.ciamlogin.com/"
  description = "CIAM Instance URL - use for AzureAd:Instance and VITE_ENTRA_INSTANCE"
}

output "known_authority" {
  value       = "${var.ciam_tenant_domain}.ciamlogin.com"
  description = "Known authority for MSAL config"
}

# Summary output for easy copy-paste
output "configuration_summary" {
  value       = <<-EOT

    ╔═══════════════════════════════════════════════════════════════════════════╗
    ║                    CIAM CONFIGURATION SUMMARY                             ║
    ╠═══════════════════════════════════════════════════════════════════════════╣
    ║ FRONTEND (.env or Aspire secrets):                                        ║
    ║   VITE_ENTRA_CLIENT_ID     = ${azuread_application.spa.client_id}
    ║   VITE_ENTRA_TENANT_ID     = ${var.ciam_tenant_id}
    ║   VITE_ENTRA_INSTANCE      = https://${var.ciam_tenant_domain}.ciamlogin.com/
    ║   VITE_ENTRA_API_CLIENT_ID = ${azuread_application.api.client_id}
    ║   VITE_ENTRA_SCOPES        = api://${var.project_name}-api/user_impersonation
    ╠═══════════════════════════════════════════════════════════════════════════╣
    ║ BACKEND (App Settings or Aspire):                                         ║
    ║   AzureAd__Instance        = https://${var.ciam_tenant_domain}.ciamlogin.com/
    ║   AzureAd__TenantId        = ${var.ciam_tenant_id}
    ║   AzureAd__ClientId        = ${azuread_application.api.client_id}
    ║   AzureAd__Audience        = api://${var.project_name}-api
    ╠═══════════════════════════════════════════════════════════════════════════╣
    ║ NEXT STEP: Add both apps to your User Flow in the Azure Portal            ║
    ║            External Identities > User flows > SignUpSignIn > Applications ║
    ╚═══════════════════════════════════════════════════════════════════════════╝

  EOT
  description = "Configuration summary for easy setup"
}
