# Environment Variables Configuration Guide

This document describes how to set up environment variables required for the Zion Reminder API in different deployment environments.

## Required Environment Variables

### Email Configuration

| Variable Name | Description | Example Value |
|---------------|-------------|--------------|
| EMAIL_PASSWORD | Password for the SMTP email account | `your-app-password` |

## Setting Up Environment Variables in Different Environments

### Local Development

For local development, use user secrets:

```powershell
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password-here"
```

### Docker

When using Docker, set environment variables in your docker-compose.yml:

```yaml
services:
  zion-api:
    image: zion-reminder-api
    environment:
      - EMAIL_PASSWORD=your-app-password
```

Or when running with Docker run:

```powershell
docker run -e EMAIL_PASSWORD=your-app-password zion-reminder-api
```

### Azure App Service

Set environment variables in the Azure Portal:

1. Go to your App Service resource
2. Navigate to "Configuration" > "Application settings"
3. Add new application setting:
   - Name: `EMAIL_PASSWORD`
   - Value: `your-app-password`
4. Click "Save"

Or use Azure CLI:

```powershell
az webapp config appsettings set --name <app-name> --resource-group <resource-group> --settings EMAIL_PASSWORD="your-app-password"
```

### Kubernetes

Set environment variables in your Kubernetes deployment YAML:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: zion-reminder-api
spec:
  template:
    spec:
      containers:
      - name: zion-reminder-api
        env:
        - name: EMAIL_PASSWORD
          valueFrom:
            secretKeyRef:
              name: zion-reminder-secrets
              key: email-password
```

Create the secret:

```powershell
kubectl create secret generic zion-reminder-secrets --from-literal=email-password="your-app-password"
```

## Security Best Practices

1. Never commit secrets to source control
2. Use managed secret stores when possible (Azure Key Vault, AWS Secrets Manager, etc.)
3. Rotate passwords regularly
4. Use service-specific accounts with minimal privileges

For more secure configurations in production environments, consider:
- Azure Key Vault integration
- Managed identities
- Secret rotation policies
