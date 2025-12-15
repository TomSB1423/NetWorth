resource "azurerm_container_registry" "acr" {
  name                = "acr${replace(var.project_name, "-", "")}${var.environment}${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true

  tags = local.common_tags
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "law-${var.project_name}-${local.resource_suffix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = local.common_tags
}

resource "azurerm_application_insights" "appinsights" {
  name                = "ai-${var.project_name}-${local.resource_suffix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  workspace_id        = azurerm_log_analytics_workspace.law.id
  application_type    = "web"

  tags = local.common_tags
}

resource "azurerm_container_app_environment" "cae" {
  name                       = "cae-${var.project_name}-${local.resource_suffix}"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id

  tags = local.common_tags
}

resource "azurerm_container_app" "functions" {
  name                         = "ca-${var.project_name}-${local.resource_suffix}"
  container_app_environment_id = azurerm_container_app_environment.cae.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  identity {
    type = "SystemAssigned"
  }

  registry {
    server               = azurerm_container_registry.acr.login_server
    username             = azurerm_container_registry.acr.admin_username
    password_secret_name = "acr-password"
  }

  secret {
    name  = "acr-password"
    value = azurerm_container_registry.acr.admin_password
  }

  secret {
    name  = "postgres-password"
    value = var.postgres_admin_password
  }

  secret {
    name  = "gocardless-secret-id"
    value = var.gocardless_secret_id
  }

  secret {
    name  = "gocardless-secret-key"
    value = var.gocardless_secret_key
  }

  secret {
    name  = "storage-connection-string"
    value = azurerm_storage_account.st.primary_connection_string
  }
  ingress {
    allow_insecure_connections = false
    external_enabled           = true
    target_port                = 80
    transport                  = "auto"

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  # Note: CORS is configured via Azure CLI after apply:
  # az containerapp ingress cors update --name <name> --resource-group <rg> \
  #   --allowed-origins "https://<frontend-url>" "http://localhost:5173" \
  #   --allowed-methods "GET" "POST" "PUT" "DELETE" "OPTIONS" \
  #   --allowed-headers "*" --allow-credentials true

  template {
    min_replicas = 0
    max_replicas = 3

    container {
      name   = "functions"
      image  = "${azurerm_container_registry.acr.login_server}/networth-functions:latest"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name        = "AzureWebJobsStorage"
        secret_name = "storage-connection-string"
      }

      env {
        name        = "ConnectionStrings__queues"
        secret_name = "storage-connection-string"
      }

      env {
        name  = "ConnectionStrings__networth-db"
        value = "Host=${azurerm_postgresql_flexible_server.psql.fqdn};Database=networth-db;Username=networthadmin;Password=${var.postgres_admin_password}"
      }

      env {
        name        = "Gocardless__SecretId"
        secret_name = "gocardless-secret-id"
      }

      env {
        name        = "Gocardless__SecretKey"
        secret_name = "gocardless-secret-key"
      }

      env {
        name  = "Frontend__Url"
        value = "https://${azurerm_static_web_app.frontend.default_host_name}"
      }

      env {
        name  = "APPLICATIONINSIGHTS_CONNECTION_STRING"
        value = azurerm_application_insights.appinsights.connection_string
      }

      env {
        name  = "FUNCTIONS_WORKER_RUNTIME"
        value = "dotnet-isolated"
      }

      env {
        name  = "AzureFunctionsJobHost__Logging__Console__IsEnabled"
        value = "true"
      }

      # Azure AD JWT Bearer authentication configuration
      env {
        name  = "AzureAd__Instance"
        value = "https://login.microsoftonline.com/"
      }

      env {
        name  = "AzureAd__TenantId"
        value = data.azurerm_client_config.current.tenant_id
      }

      env {
        name  = "AzureAd__ClientId"
        value = azuread_application.api.client_id
      }

      env {
        name  = "AzureAd__Audience"
        value = azuread_application.api.client_id
      }
    }
  }

  tags = local.common_tags
}
