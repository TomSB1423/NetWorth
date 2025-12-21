# Firebase Authentication with Google Sign-In.
# See AUTHENTICATION.md for setup instructions.
#
# Secrets in Key Vault:
#   firebase-api-key         - Web API key for frontend
#   firebase-service-account - Base64-encoded service account JSON

data "azuread_client_config" "current" {}


