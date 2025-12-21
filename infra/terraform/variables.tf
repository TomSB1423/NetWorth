# =============================================================================
# Variables
# =============================================================================

# -----------------------------------------------------------------------------
# General Settings
# -----------------------------------------------------------------------------

variable "location" {
  description = "The Azure region to deploy resources into"
  type        = string
  default     = "uksouth"
}

variable "environment" {
  description = "The environment name (e.g., dev, staging, prod)"
  type        = string
  default     = "dev"

  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be one of: dev, staging, prod"
  }
}

variable "project_name" {
  description = "The name of the project"
  type        = string
  default     = "networth"
}

# -----------------------------------------------------------------------------
# Database Credentials
# -----------------------------------------------------------------------------

variable "postgres_admin_password" {
  description = "The password for the PostgreSQL admin user"
  type        = string
  sensitive   = true
}

# -----------------------------------------------------------------------------
# GoCardless API Credentials
# -----------------------------------------------------------------------------

variable "gocardless_secret_id" {
  description = "The GoCardless Secret ID for bank account aggregation"
  type        = string
  sensitive   = true
}

variable "gocardless_secret_key" {
  description = "The GoCardless Secret Key for bank account aggregation"
  type        = string
  sensitive   = true
}

# -----------------------------------------------------------------------------
# CIAM / Entra External ID Configuration
# These values come from the terraform-ciam deployment
# -----------------------------------------------------------------------------

variable "ciam_tenant_domain" {
  description = "The CIAM tenant domain (e.g., 'yourname' for yourname.ciamlogin.com)"
  type        = string
}

variable "ciam_tenant_id" {
  description = "The CIAM (External ID) Tenant ID"
  type        = string
}

variable "ciam_api_client_id" {
  description = "The API Application (Client) ID from CIAM - used for AzureAd:ClientId and AzureAd:Audience"
  type        = string
}

variable "ciam_spa_client_id" {
  description = "The SPA Application (Client) ID from CIAM - passed to frontend"
  type        = string
}
