# =============================================================================
# Authentication Configuration (Minimal)
# =============================================================================
# Authentication is handled via Entra External ID (CIAM) which is deployed
# separately in the terraform-ciam folder. This file only contains the
# data source for the current Azure tenant and any remaining Azure AD
# resources needed for internal Azure operations.
#
# For CIAM app registrations, see: ../terraform-ciam/
# =============================================================================

data "azuread_client_config" "current" {}

# Note: The previous Azure AD app registrations for API and Frontend have been
# removed. Authentication is now handled by Entra External ID (CIAM) which
# provides:
# - Consumer-facing sign-up/sign-in flows
# - Self-service password reset
# - Social identity provider integration
# - Customizable branding
#
# The CIAM app registrations are managed in ../terraform-ciam/ and the
# client IDs are passed to this configuration via variables:
# - var.ciam_api_client_id
# - var.ciam_spa_client_id
# - var.ciam_tenant_id
# - var.ciam_tenant_domain


