# =============================================================================
# CIAM Variables
# =============================================================================
# Configuration for Entra External ID (CIAM) tenant app registrations.
# These app registrations must be created in a separate CIAM tenant,
# not in the main Azure AD tenant.
# =============================================================================

variable "ciam_tenant_id" {
  description = "The CIAM (External ID) Tenant ID"
  type        = string
}

variable "ciam_tenant_domain" {
  description = "The CIAM tenant domain (e.g., 'yourname' for yourname.ciamlogin.com)"
  type        = string
}

variable "project_name" {
  description = "The name of the project"
  type        = string
  default     = "networth"
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

variable "spa_redirect_uris" {
  description = "Redirect URIs for the SPA (include localhost for dev, and deployed URLs)"
  type        = list(string)
  default = [
    "http://localhost:5173/", # Vite dev server
    "http://localhost:3000/"  # Alternative dev port
  ]
}
