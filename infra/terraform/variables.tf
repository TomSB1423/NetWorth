variable "location" {
  description = "The Azure region to deploy resources into"
  type        = string
  default     = "uksouth"
}

variable "environment" {
  description = "The environment name (e.g., dev, prod)"
  type        = string
  default     = "dev"
}

variable "project_name" {
  description = "The name of the project"
  type        = string
  default     = "networth"
}

variable "postgres_admin_password" {
  description = "The password for the PostgreSQL admin user"
  type        = string
  sensitive   = true
}

variable "gocardless_secret_id" {
  description = "The GoCardless Secret ID"
  type        = string
  sensitive   = true
}

variable "gocardless_secret_key" {
  description = "The GoCardless Secret Key"
  type        = string
  sensitive   = true
}
