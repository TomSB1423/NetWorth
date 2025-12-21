# =============================================================================
# CIAM App Registrations for Networth
# =============================================================================
# This Terraform configuration creates the identity contracts in the
# External ID (CIAM) tenant. These are separate from the infrastructure
# deployed in the main Azure tenant.
#
# NOTE: This configuration preserves the existing working setup with
# custom API scopes (user_impersonation).
# =============================================================================

data "azuread_client_config" "current" {}

# -----------------------------------------------------------------------------
# API Application (networth-api)
# -----------------------------------------------------------------------------
# This application represents the backend API with a custom scope.

resource "random_uuid" "user_impersonation_scope" {}

resource "azuread_application" "api" {
  display_name     = "${var.project_name}-api"
  owners           = [data.azuread_client_config.current.object_id]
  sign_in_audience = "AzureADMyOrg"

  # API identifier URI
  identifier_uris = ["api://${var.project_name}-api"]

  api {
    requested_access_token_version = 2

    oauth2_permission_scope {
      admin_consent_description  = "Allow the application to access Networth API on behalf of the signed-in user."
      admin_consent_display_name = "Access Networth API"
      enabled                    = true
      id                         = random_uuid.user_impersonation_scope.result
      type                       = "User"
      user_consent_description   = "Allow the application to access Networth API on your behalf."
      user_consent_display_name  = "Access Networth API"
      value                      = "user_impersonation"
    }
  }

  # Claims to include in tokens
  optional_claims {
    access_token {
      name = "email"
    }
    access_token {
      name = "preferred_username"
    }
    id_token {
      name = "email"
    }
    id_token {
      name = "preferred_username"
    }
  }

  tags = [var.project_name, "api", "ManagedByTerraform"]

  lifecycle {
    ignore_changes = [identifier_uris]
  }
}

resource "azuread_service_principal" "api" {
  client_id                    = azuread_application.api.client_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
}

# -----------------------------------------------------------------------------
# SPA Application (networth-spa)
# -----------------------------------------------------------------------------
# This application represents the frontend SPA. It is configured as a
# public client (no secrets) with redirect URIs for the authentication flow.
# The SPA requests the API's user_impersonation scope.

resource "azuread_application" "spa" {
  display_name     = "${var.project_name}-spa"
  owners           = [data.azuread_client_config.current.object_id]
  sign_in_audience = "AzureADMyOrg"

  # SPA configuration - enables Authorization Code flow with PKCE
  single_page_application {
    redirect_uris = var.spa_redirect_uris
  }

  # Request access to the API
  required_resource_access {
    resource_app_id = azuread_application.api.client_id

    resource_access {
      id   = random_uuid.user_impersonation_scope.result
      type = "Scope"
    }
  }

  # Request Microsoft Graph permissions (basic profile info)
  required_resource_access {
    resource_app_id = "00000003-0000-0000-c000-000000000000" # Microsoft Graph

    resource_access {
      id   = "e1fe6dd8-ba31-4d61-89e7-88639da4683d" # User.Read
      type = "Scope"
    }
    resource_access {
      id   = "37f7f235-527c-4136-accd-4a02d197296e" # openid
      type = "Scope"
    }
    resource_access {
      id   = "7427e0e9-2fba-42fe-b0c0-848c9e6a8182" # offline_access
      type = "Scope"
    }
  }

  # Claims to include in tokens
  optional_claims {
    id_token {
      name = "email"
    }
    id_token {
      name = "preferred_username"
    }
  }

  tags = [var.project_name, "spa", "ManagedByTerraform"]
}

resource "azuread_service_principal" "spa" {
  client_id                    = azuread_application.spa.client_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
}

# -----------------------------------------------------------------------------
# Admin Consent Grant
# -----------------------------------------------------------------------------
# Grant Microsoft Graph permissions

data "azuread_service_principal" "msgraph" {
  client_id = "00000003-0000-0000-c000-000000000000" # Microsoft Graph
}

resource "azuread_service_principal_delegated_permission_grant" "spa_to_graph" {
  service_principal_object_id          = azuread_service_principal.spa.object_id
  resource_service_principal_object_id = data.azuread_service_principal.msgraph.object_id
  claim_values                         = ["openid", "offline_access", "User.Read"]
}
