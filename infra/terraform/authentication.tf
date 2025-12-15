data "azuread_client_config" "current" {}

resource "azuread_application" "api" {
  display_name     = "${var.project_name}-api-${var.environment}"
  owners           = [data.azuread_client_config.current.object_id]
  sign_in_audience = "AzureADMyOrg"
  identifier_uris  = ["api://${var.project_name}-api-${var.environment}"]

  api {
    requested_access_token_version = 2

    oauth2_permission_scope {
      admin_consent_description  = "Allow the application to access ${var.project_name} API on behalf of the signed-in user."
      admin_consent_display_name = "Access ${var.project_name} API"
      enabled                    = true
      id                         = random_uuid.user_impersonation_scope.result
      type                       = "User"
      user_consent_description   = "Allow the application to access ${var.project_name} API on your behalf."
      user_consent_display_name  = "Access ${var.project_name} API"
      value                      = "user_impersonation"
    }
  }

  optional_claims {
    id_token {
      name = "email"
    }
    id_token {
      name = "preferred_username"
    }
  }

  tags = ["${var.project_name}", var.environment, "ManagedByTerraform"]

  lifecycle {
    ignore_changes = [web, identifier_uris]
  }
}

resource "random_uuid" "user_impersonation_scope" {}

resource "azuread_application_password" "api" {
  application_id = azuread_application.api.id
  display_name   = "${var.project_name}-api-secret"
  end_date       = timeadd(timestamp(), "8760h") # 1 year

  lifecycle {
    ignore_changes = [end_date]
  }
}

resource "azuread_service_principal" "api" {
  client_id                    = azuread_application.api.client_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
}

resource "azuread_application" "frontend" {
  display_name     = "${var.project_name}-frontend-${var.environment}"
  owners           = [data.azuread_client_config.current.object_id]
  sign_in_audience = "AzureADMyOrg"

  single_page_application {
    redirect_uris = [
      "https://${azurerm_static_web_app.frontend.default_host_name}/",
      "http://localhost:5173/"
    ]
  }

  required_resource_access {
    resource_app_id = azuread_application.api.client_id

    resource_access {
      id   = random_uuid.user_impersonation_scope.result
      type = "Scope"
    }
  }

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

  optional_claims {
    id_token {
      name = "email"
    }
    id_token {
      name = "preferred_username"
    }
  }

  tags = ["${var.project_name}", var.environment, "ManagedByTerraform"]
}

resource "azuread_service_principal" "frontend" {
  client_id                    = azuread_application.frontend.client_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
}

# CORS configuration for the Container App
# Allows frontend and localhost to make cross-origin requests
resource "terraform_data" "container_app_cors" {
  triggers_replace = [
    azurerm_container_app.functions.id,
    azurerm_static_web_app.frontend.default_host_name
  ]

  provisioner "local-exec" {
    command = <<-EOT
      az containerapp ingress cors update \
        --name ${azurerm_container_app.functions.name} \
        --resource-group ${azurerm_resource_group.rg.name} \
        --allowed-origins "https://${azurerm_static_web_app.frontend.default_host_name}" "http://localhost:5173" \
        --allowed-methods "GET" "POST" "PUT" "DELETE" "OPTIONS" \
        --allowed-headers "*" \
        --allow-credentials true
    EOT
  }

  depends_on = [
    azurerm_container_app.functions,
    azurerm_static_web_app.frontend
  ]
}


