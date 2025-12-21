# General

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

# Database

variable "postgres_admin_password" {
  description = "The password for the PostgreSQL admin user"
  type        = string
  sensitive   = true
}

# GoCardless (https://bankaccountdata.gocardless.com/)

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

# Firebase Authentication (https://console.firebase.google.com/)

variable "firebase_project_id" {
  description = "The Firebase project ID"
  type        = string
}

variable "firebase_api_key" {
  description = "The Firebase Web API Key (for frontend)"
  type        = string
  sensitive   = true
}

variable "firebase_auth_domain" {
  description = "The Firebase Auth domain (e.g., 'project-id.firebaseapp.com')"
  type        = string
}

variable "firebase_storage_bucket" {
  description = "The Firebase Storage bucket (e.g., 'project-id.appspot.com')"
  type        = string
}

variable "firebase_messaging_sender_id" {
  description = "The Firebase Cloud Messaging sender ID"
  type        = string
}

variable "firebase_app_id" {
  description = "The Firebase App ID"
  type        = string
}
