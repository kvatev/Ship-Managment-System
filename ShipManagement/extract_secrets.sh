#!/bin/bash
# Ensure GitHub CLI is installed and authenticated

# Fetch secrets
SMTP_SERVER_SECRET_FOR_PASSWORD=$(gh secret list --repo owner/repo --jq '.[] | select(.name == "SMTP_SERVER_SECRET_FOR_PASSWORD") | .value')
AZURE_SECRET_FOR_PASSWORD=$(gh secret list --repo owner/repo --jq '.[] | select(.name == "AZURE_SECRET_FOR_PASSWORD") | .value')

# Export secrets to environment
export SMTP_SERVER_SECRET_FOR_PASSWORD
export AZURE_SECRET_FOR_PASSWORD