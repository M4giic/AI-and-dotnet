# Event Management Platform - Deployment Guide

## Overview

This document provides comprehensive instructions for deploying the Event Management Platform to different environments. The platform consists of a Blazor WebAssembly frontend, ASP.NET Core backend API, and SQL Server database.

## System Requirements

### Development Environment

- Windows 10/11, macOS, or Linux
- .NET 7.0 SDK or later
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider
- SQL Server (LocalDB, Express, or higher)
- Node.js 14+ (for frontend tooling)
- Git

### Production Environment

- Windows Server 2019/2022 or Linux (Ubuntu 20.04 LTS or higher)
- .NET 7.0 Runtime
- SQL Server 2019 or higher
- Azure App Service (recommended)
- Azure SQL Database (recommended)
- Azure Storage Account (for media files)
- Azure Redis Cache (optional, for improved performance)

## Deployment Options

### 1. Azure App Service Deployment

#### Prerequisites

- Azure subscription
- Azure App Service plan
- Azure SQL Database
- Azure Storage Account
- Azure Key Vault (for secrets)

#### Deployment Steps

1. **Prepare Azure Resources**
    
    - Create a Resource Group
    - Create an App Service Plan (Standard tier or higher recommended)
    - Create an Azure SQL Database
    - Create a Storage Account for media files
    - Create a Key Vault for secrets management
2. **Configure Azure SQL Database**
    
    - Set up firewall rules
    - Create a database user with appropriate permissions
    - Note connection string for later use
3. **Configure Storage Account**
    
    - Create blob containers:
        - `event-images`
        - `speaker-profiles`
        - `session-materials`
    - Set up CORS policies for the web application
    - Generate SAS tokens or use Managed Identity
4. **Set Up Key Vault**
    
    - Add secrets:
        - `DatabaseConnection` - SQL connection string
        - `StorageConnection` - Storage account connection string
        - `JwtKey` - Secret key for JWT token generation
        - `SendGridApiKey` - API key for email service
5. **Configure App Service**
    
    - Enable Managed Identity
    - Grant Key Vault access to Managed Identity
    - Configure application settings:
        - `ASPNETCORE_ENVIRONMENT`: Production
        - `KeyVault__Endpoint`: Key Vault URI
        - `Storage__BaseUrl`: Storage account base URL
6. **Deploy Backend API**
    
    - Publish from Visual Studio:
        - Right-click on API project > Publish
        - Select Azure App Service target
        - Configure deployment settings
    - Or use Azure DevOps CI/CD pipeline:
        - Configure build pipeline using azure-pipelines.yml
        - Set up release pipeline to deploy to App Service
7. **Deploy Frontend Application**
    
    - Build Blazor WebAssembly app for production:
        
        ```
        dotnet publish -c Release
        ```
        
    - Deploy to App Service:
        - Create a separate App Service or deploy to a subdirectory
        - Configure API base URL in appsettings.json
8. **Run Database Migrations**
    
    - Using EF Core CLI:
        
        ```
        dotnet ef database update --connection "connection-string"
        ```
        
    - Or enable automatic migrations in app startup
9. **Verify Deployment**
    
    - Check Application Insights for errors
    - Verify database connection
    - Test authentication flow
    - Confirm storage access

### 2. Docker Deployment

#### Prerequisites

- Docker and Docker Compose
- Container registry (Azure Container Registry, Docker Hub, etc.)
- Host environment with Docker support

#### Deployment Steps

1. **Prepare Docker Images**
    
    - Build API image:
        
        ```
        docker build -t eventmanagement/api:latest -f src/Api/Dockerfile .
        ```
        
    - Build WebAssembly image:
        
        ```
        docker build -t eventmanagement/web:latest -f src/Web/Dockerfile .
        ```
        
2. **Push to Container Registry**
    
    ```
    docker push eventmanagement/api:latest
    docker push eventmanagement/web:latest
    ```
    
3. **Prepare Docker Compose File**
    
    ```
    # docker-compose.yml example structure
    version: '3.8'
    services:
      api:
        image: eventmanagement/api:latest
        environment:
          - ASPNETCORE_ENVIRONMENT=Production
          - ConnectionStrings__DefaultConnection=...
        ports:
          - "5001:80"
      web:
        image: eventmanagement/web:latest
        environment:
          - ApiBaseUrl=http://api
        ports:
          - "5000:80"
      db:
        image: mcr.microsoft.com/mssql/server:2019-latest
        environment:
          - ACCEPT_EULA=Y
          - SA_PASSWORD=...
        volumes:
          - sqldata:/var/opt/mssql
    
    volumes:
      sqldata:
    ```
    
4. **Deploy with Docker Compose**
    
    ```
    docker-compose up -d
    ```
    
5. **Run Database Migrations**
    
    ```
    docker exec -it eventmanagement_api_1 dotnet ef database update
    ```
    
6. **Configure Reverse Proxy**
    
    - Set up Nginx or other reverse proxy
    - Configure SSL termination
    - Set up appropriate headers

### 3. On-Premises Deployment

#### Prerequisites

- Windows Server 2019/2022 with IIS
- SQL Server 2019 or higher
- .NET 7.0 Runtime
- ASP.NET Core Hosting Bundle

#### Deployment Steps

1. **Prepare Server**
    
    - Install required software
    - Configure IIS with ASP.NET Core Hosting Bundle
    - Set up SQL Server instance
    - Create application database
2. **Configure Database**
    
    - Run SQL scripts to create schema
    - Set up database user with appropriate permissions
    - Configure connection settings
3. **Prepare Application Files**
    
    - Build API project:
        
        ```
        dotnet publish -c Release -o c:\deployments\eventmanagement\api
        ```
        
    - Build WebAssembly project:
        
        ```
        dotnet publish -c Release -o c:\deployments\eventmanagement\web
        ```
        
4. **Configure IIS**
    
    - Create application pools
    - Create websites/applications
    - Configure bindings and host names
    - Set up authentication and authorization
5. **Configure File Storage**
    
    - Set up a file share for media storage
    - Configure appropriate permissions
    - Update application settings
6. **Configure Application Settings**
    
    - Update appsettings.json for both applications
    - Configure connection strings
    - Set environment variables
7. **Run Database Migrations**
    
    - Using EF Core CLI or migration scripts
8. **Configure Windows Services**
    
    - Set up background processing service
    - Configure service dependencies
    - Set appropriate startup type

## Environment-Specific Configuration

### Development

```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventManagement;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Storage": {
    "Type": "FileSystem",
    "BasePath": "C:\\EventManagementStorage"
  },
  "Authentication": {
    "JwtKey": "dev-signing-key-should-be-very-long-and-secure-for-development",
    "JwtIssuer": "https://localhost:5001",
    "JwtAudience": "https://localhost:5000",
    "JwtExpiryMinutes": 60
  }
}
```

### Staging

```json
// appsettings.Staging.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=staging-sql-server;Database=EventManagement;User Id=appuser;Password=StrongP@ssw0rd;"
  },
  "Storage": {
    "Type": "AzureBlobStorage",
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=stagingaccount;..."
  },
  "Authentication": {
    "JwtKey": "staging-key-retrieved-from-key-vault",
    "JwtIssuer": "https://api-staging.eventmanagement.example",
    "JwtAudience": "https://staging.eventmanagement.example",
    "JwtExpiryMinutes": 60
  }
}
```

### Production 

json

```json
// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=production-sql-server.database.windows.net;Database=EventManagement;User Id=appuser;Password=StrongP@ssw0rd;"
  },
  "Storage": {
    "Type": "AzureBlobStorage",
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=productionaccount;..."
  },
  "Authentication": {
    "JwtKey": "production-key-retrieved-from-key-vault",
    "JwtIssuer": "https://api.eventmanagement.example",
    "JwtAudience": "https://eventmanagement.example",
    "JwtExpiryMinutes": 30
  },
  "Caching": {
    "RedisConnection": "production-redis-cache.redis.cache.windows.net:6380,password=...,ssl=True,abortConnect=False"
  },
  "ApplicationInsights": {
    "InstrumentationKey": "production-app-insights-key"
  }
}
```

## Configuration Management

### Configuration Sources

The application uses the following configuration sources, listed in order of precedence:

1. Environment Variables
2. Azure Key Vault (in Azure environments)
3. User Secrets (Development only)
4. appsettings.{Environment}.json
5. appsettings.json

### Secret Management

Sensitive configuration values should never be stored in source control. Use the following approaches:

- **Development**: Use .NET Secret Manager
    
    ```
    dotnet user-secrets set "Authentication:JwtKey" "your-development-key"
    ```
    
- **Production**: Use Azure Key Vault
    
    csharp
    
    ```csharp
    // Program.cs configuration
    if (context.HostingEnvironment.IsProduction())
    {
        config.AddAzureKeyVault(
            new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
            new DefaultAzureCredential());
    }
    ```
    

### Feature Flags

The application supports feature flags for controlled feature rollout:

json

```json
// Example feature flag configuration
{
  "FeatureManagement": {
    "AdvancedSessionScheduling": true,
    "PaymentProcessing": true,
    "EmailNotifications": true,
    "SmsNotifications": false,
    "VirtualEventSupport": true,
    "Beta": {
      "EnabledFor": [
        {
          "Name": "Percentage",
          "Parameters": {
            "Value": 20
          }
        }
      ]
    }
  }
}
```

## Scaling Considerations

### Horizontal Scaling

The application is designed for horizontal scaling:

- Stateless API allows for multiple instances
- Session state stored in distributed cache
- Database connection pooling configured appropriately
- Blob storage for media files accessible from all instances

### Vertical Scaling

Considerations for vertical scaling:

- Increase App Service plan size for CPU/memory-intensive operations
- Scale up SQL Database tier for higher database throughput
- Monitor resource utilization to identify bottlenecks

### Database Scaling

Options for database scaling:

- Implement read replicas for read-heavy workloads
- Consider sharding for very large deployments
- Use SQL elastic pools for cost-effective multi-tenant scenarios

### Regional Deployment

For global access:

- Deploy to multiple Azure regions
- Use Traffic Manager for routing
- Implement geo-replication for databases
- Configure CDN for static assets

## Monitoring and Operations

### Health Checks

The application includes health check endpoints:

- `/health`: Overall application health
- `/health/ready`: Readiness check
- `/health/live`: Liveness check
- `/health/db`: Database connectivity
- `/health/storage`: Storage connectivity

### Metrics Collection

Key metrics collected:

- API response times
- Database query performance
- Authentication success/failure rates
- Registration throughput
- Error rates by endpoint

### Logging Configuration

Structured logging with Serilog:

json

```json
// Logging configuration
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/event-management-.log",
          "rollingInterval": "Day"
        }
      },
      {
        "Name": "ApplicationInsights",
        "Args": {
          "instrumentationKey": "app-insights-key",
          "telemetryConverter": "Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

### Alerting

Configure alerts for:

- High error rates
- Slow response times
- Database connection failures
- Storage access issues
- Authentication failures
- Low disk space
- High CPU/memory utilization

## Backup and Disaster Recovery

### Database Backups

- SQL automated backups
- Point-in-time restore capability
- Geo-redundant backup storage
- Regular backup testing

### Application Backups

- Infrastructure-as-Code templates
- Configuration backups
- CI/CD pipeline artifacts
- Deployment rollback capability

### Disaster Recovery Plan

1. **Assessment**:
    - Identify affected components
    - Determine impact severity
    - Estimate recovery time
2. **Communication**:
    - Notify stakeholders
    - Provide estimated resolution time
    - Establish communication channels
3. **Recovery**:
    - Restore database from backup
    - Redeploy application if necessary
    - Verify system functionality
    - Validate data integrity
4. **Post-Incident**:
    - Document incident
    - Perform root cause analysis
    - Implement preventive measures
    - Update recovery procedures

## CI/CD Pipeline

### Azure DevOps Configuration

yaml

```yaml
# azure-pipelines.yml
trigger:
- main
- release/*

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetSdkVersion: '7.0.x'

stages:
- stage: Build
  jobs:
  - job: Build
    steps:
    - task: UseDotNet@2
      inputs:
        version: $(dotnetSdkVersion)
        includePreviewVersions: false

    - task: DotNetCoreCLI@2
      displayName: Restore
      inputs:
        command: restore
        projects: '**/*.csproj'

    - task: DotNetCoreCLI@2
      displayName: Build
      inputs:
        command: build
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration)'

    - task: DotNetCoreCLI@2
      displayName: Test
      inputs:
        command: test
        projects: '**/*Tests/*.csproj'
        arguments: '--configuration $(buildConfiguration) --collect "Code coverage"'

    - task: DotNetCoreCLI@2
      displayName: Publish API
      inputs:
        command: publish
        publishWebProjects: false
        projects: '**/Api.csproj'
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/api'
        zipAfterPublish: true

    - task: DotNetCoreCLI@2
      displayName: Publish Web
      inputs:
        command: publish
        publishWebProjects: false
        projects: '**/Web.csproj'
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/web'
        zipAfterPublish: true

    - task: PublishBuildArtifacts@1
      displayName: 'Publish Artifacts'
      inputs:
        pathtoPublish: '$(Build.ArtifactStagingDirectory)'
        artifactName: 'drop'

- stage: Deploy_Dev
  dependsOn: Build
  condition: succeeded()
  jobs:
  - deployment: DeployAPI
    environment: development
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureRmWebAppDeployment@4
            inputs:
              ConnectionType: 'AzureRM'
              appType: 'webApp'
              WebAppName: 'dev-eventmanagement-api'
              packageForLinux: '$(Pipeline.Workspace)/drop/api/*.zip'

  - deployment: DeployWeb
    environment: development
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureRmWebAppDeployment@4
            inputs:
              ConnectionType: 'AzureRM'
              appType: 'webApp'
              WebAppName: 'dev-eventmanagement-web'
              packageForLinux: '$(Pipeline.Workspace)/drop/web/*.zip'
```

### GitHub Actions Configuration

yaml

```yaml
# .github/workflows/dotnet.yml
name: .NET Build and Deploy

on:
  push:
    branches: [ main, release/* ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v2
      with:
        dotnet-version: '7.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --configuration Release --no-restore
      
    - name: Test
      run: dotnet test --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
    - name: Publish API
      run: dotnet publish src/Api/Api.csproj -c Release -o publish/api
      
    - name: Publish Web
      run: dotnet publish src/Web/Web.csproj -c Release -o publish/web
      
    - name: Upload API artifact
      uses: actions/upload-artifact@v3
      with:
        name: api
        path: publish/api
        
    - name: Upload Web artifact
      uses: actions/upload-artifact@v3
      with:
        name: web
        path: publish/web

  deploy-dev:
    needs: build
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    
    steps:
    - name: Download API artifact
      uses: actions/download-artifact@v3
      with:
        name: api
        path: api
        
    - name: Download Web artifact
      uses: actions/download-artifact@v3
      with:
        name: web
        path: web
        
    - name: Deploy API to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'dev-eventmanagement-api'
        publish-profile: ${{ secrets.DEV_API_PUBLISH_PROFILE }}
        package: api
        
    - name: Deploy Web to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'dev-eventmanagement-web'
        publish-profile: ${{ secrets.DEV_WEB_PUBLISH_PROFILE }}
        package: web
```

## Infrastructure as Code

### Azure Resource Management Template

json

```json
// ARM template for main infrastructure
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "environment": {
      "type": "string",
      "allowedValues": ["dev", "test", "staging", "prod"],
      "defaultValue": "dev"
    },
    "sqlAdminLogin": {
      "type": "string",
      "metadata": {
        "description": "The administrator username for the SQL Server."
      }
    },
    "sqlAdminPassword": {
      "type": "securestring",
      "metadata": {
        "description": "The administrator password for the SQL Server."
      }
    }
  },
  "variables": {
    "prefix": "[concat('eventmgmt-', parameters('environment'))]",
    "appServicePlanName": "[concat(variables('prefix'), '-plan')]",
    "apiAppName": "[concat(variables('prefix'), '-api')]",
    "webAppName": "[concat(variables('prefix'), '-web')]",
    "sqlServerName": "[concat(variables('prefix'), '-sql')]",
    "sqlDatabaseName": "EventManagement",
    "storageAccountName": "[replace(concat(variables('prefix'), 'storage'), '-', '')]",
    "keyVaultName": "[concat(variables('prefix'), '-kv')]",
    "appInsightsName": "[concat(variables('prefix'), '-ai')]"
  },
  "resources": [
    // App Service Plan
    {
      "type": "Microsoft.Web/serverfarms",
      "apiVersion": "2021-02-01",
      "name": "[variables('appServicePlanName')]",
      "location": "[resourceGroup().location]",
      "sku": {
        "name": "P1v2",
        "tier": "PremiumV2",
        "size": "P1v2",
        "family": "Pv2",
        "capacity": 1
      }
    },
    
    // API App Service
    {
      "type": "Microsoft.Web/sites",
      "apiVersion": "2021-02-01",
      "name": "[variables('apiAppName')]",
      "location": "[resourceGroup().location]",
      "dependsOn": [
        "[resourceId('Microsoft.Web/serverfarms', variables('appServicePlanName'))]"
      ],
      "properties": {
        "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', variables('appServicePlanName'))]",
        "httpsOnly": true,
        "siteConfig": {
          "netFrameworkVersion": "v7.0",
          "appSettings": [
            {
              "name": "ASPNETCORE_ENVIRONMENT",
              "value": "[parameters('environment')]"
            },
            {
              "name": "APPINSIGHTS_INSTRUMENTATIONKEY",
              "value": "[reference(resourceId('Microsoft.Insights/components', variables('appInsightsName'))).InstrumentationKey]"
            },
            {
              "name": "KeyVault__Endpoint",
              "value": "[concat('https://', variables('keyVaultName'), '.vault.azure.net/')]"
            }
          ]
        }
      },
      "identity": {
        "type": "SystemAssigned"
      }
    },
    
    // Web App Service
    {
      "type": "Microsoft.Web/sites",
      "apiVersion": "2021-02-01",
      "name": "[variables('webAppName')]",
      "location": "[resourceGroup().location]",
      "dependsOn": [
        "[resourceId('Microsoft.Web/serverfarms', variables('appServicePlanName'))]"
      ],
      "properties": {
        "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', variables('appServicePlanName'))]",
        "httpsOnly": true,
        "siteConfig": {
          "appSettings": [
            {
              "name": "ApiBaseUrl",
              "value": "[concat('https://', variables('apiAppName'), '.azurewebsites.net')]"
            }
          ]
        }
      }
    },
    
    // SQL Server
    {
      "type": "Microsoft.Sql/servers",
      "apiVersion": "2021-02-01-preview",
      "name": "[variables('sqlServerName')]",
      "location": "[resourceGroup().location]",
      "properties": {
        "administratorLogin": "[parameters('sqlAdminLogin')]",
        "administratorLoginPassword": "[parameters('sqlAdminPassword')]",
        "version": "12.0"
      },
      "resources": [
        {
          "type": "databases",
          "apiVersion": "2021-02-01-preview",
          "name": "[variables('sqlDatabaseName')]",
          "location": "[resourceGroup().location]",
          "dependsOn": [
            "[resourceId('Microsoft.Sql/servers', variables('sqlServerName'))]"
          ],
          "sku": {
            "name": "Standard",
            "tier": "Standard"
          }
        },
        {
          "type": "firewallrules",
          "apiVersion": "2021-02-01-preview",
          "name": "AllowAllAzureIPs",
          "dependsOn": [
            "[resourceId('Microsoft.Sql/servers', variables('sqlServerName'))]"
          ],
          "properties": {
            "startIpAddress": "0.0.0.0",
            "endIpAddress": "0.0.0.0"
          }
        }
      ]
    },
    
    // Storage Account
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2021-04-01",
      "name": "[variables('storageAccountName')]",
      "location": "[resourceGroup().location]",
      "sku": {
        "name": "Standard_LRS"
      },
      "kind": "StorageV2",
      "properties": {
        "supportsHttpsTrafficOnly": true,
        "allowBlobPublicAccess": false
      }
    },
    
    // Key Vault
    {
      "type": "Microsoft.KeyVault/vaults",
      "apiVersion": "2021-04-01-preview",
      "name": "[variables('keyVaultName')]",
      "location": "[resourceGroup().location]",
      "properties": {
        "enabledForDeployment": false,
        "enabledForTemplateDeployment": true,
        "enabledForDiskEncryption": false,
        "tenantId": "[subscription().tenantId]",
        "accessPolicies": [],
        "sku": {
          "name": "standard",
          "family": "A"
        }
      }
    },
    
    // Application Insights
    {
      "type": "Microsoft.Insights/components",
      "apiVersion": "2020-02-02",
      "name": "[variables('appInsightsName')]",
      "location": "[resourceGroup().location]",
      "kind": "web",
      "properties": {
        "Application_Type": "web",
        "Request_Source": "rest"
      }
    }
  ],
  "outputs": {
    "apiUrl": {
      "type": "string",
      "value": "[concat('https://', variables('apiAppName'), '.azurewebsites.net')]"
    },
    "webUrl": {
      "type": "string",
      "value": "[concat('https://', variables('webAppName'), '.azurewebsites.net')]"
    },
    "sqlServerFqdn": {
      "type": "string",
      "value": "[reference(resourceId('Microsoft.Sql/servers', variables('sqlServerName'))).fullyQualifiedDomainName]"
    },
    "storageAccountName": {
      "type": "string",
      "value": "[variables('storageAccountName')]"
    },
    "keyVaultUri": {
      "type": "string",
      "value": "[reference(resourceId('Microsoft.KeyVault/vaults', variables('keyVaultName'))).vaultUri]"
    }
  }
}
```

### Terraform Configuration

hcl

```hcl
# Main Terraform file for Event Management Platform

provider "azurerm" {
  features {}
}

variable "environment" {
  description = "Environment name (dev, test, staging, prod)"
  default     = "dev"
}

variable "location" {
  description = "Azure region"
  default     = "East US"
}

variable "sql_admin_login" {
  description = "SQL Server admin username"
}

variable "sql_admin_password" {
  description = "SQL Server admin password"
  sensitive   = true
}

locals {
  prefix               = "eventmgmt-${var.environment}"
  app_service_plan_name = "${local.prefix}-plan"
  api_app_name         = "${local.prefix}-api"
  web_app_name         = "${local.prefix}-web"
  sql_server_name      = "${local.prefix}-sql"
  sql_database_name    = "EventManagement"
  storage_account_name = replace("${local.prefix}storage", "-", "")
  key_vault_name       = "${local.prefix}-kv"
  app_insights_name    = "${local.prefix}-ai"
}

resource "azurerm_resource_group" "main" {
  name     = "${local.prefix}-rg"
  location = var.location
}

# App Service Plan
resource "azurerm_app_service_plan" "main" {
  name                = local.app_service_plan_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  
  sku {
    tier = "PremiumV2"
    size = "P1v2"
  }
}

# API App Service
resource "azurerm_app_service" "api" {
  name                = local.api_app_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  app_service_plan_id = azurerm_app_service_plan.main.id
  
  site_config {
    dotnet_framework_version = "v7.0"
    http2_enabled            = true
  }
  
  app_settings = {
    "ASPNETCORE_ENVIRONMENT"          = var.environment
    "APPINSIGHTS_INSTRUMENTATIONKEY"  = azurerm_application_insights.main.instrumentation_key
    "KeyVault__Endpoint"              = "https://${azurerm_key_vault.main.name}.vault.azure.net/"
  }
  
  identity {
    type = "SystemAssigned"
  }
}

# Web App Service
resource "azurerm_app_service" "web" {
  name                = local.web_app_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  app_service_plan_id = azurerm_app_service_plan.main.id
  
  app_settings = {
    "ApiBaseUrl" = "https://${azurerm_app_service.api.default_site_hostname}"
  }
}

# SQL Server
resource "azurerm_sql_server" "main" {
  name                         = local.sql_server_name
  location                     = azurerm_resource_group.main.location
  resource_group_name          = azurerm_resource_group.main.name
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
}

# SQL Database
resource "azurerm_sql_database" "main" {
  name                = local.sql_database_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  server_name         = azurerm_sql_server.main.name
  edition             = "Standard"
}

# SQL Firewall rule for Azure services
resource "azurerm_sql_firewall_rule" "azure_services" {
  name                = "AllowAllAzureIPs"
  resource_group_name = azurerm_resource_group.main.name
  server_name         = azurerm_sql_server.main.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

# Storage Account
resource "azurerm_storage_account" "main" {
  name                     = local.storage_account_name
  location                 = azurerm_resource_group.main.location
  resource_group_name      = azurerm_resource_group.main.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
  
  blob_properties {
    cors_rule {
      allowed_origins    = ["https://${azurerm_app_service.web.default_site_hostname}"]
      allowed_methods    = ["GET", "POST", "PUT"]
      allowed_headers    = ["*"]
      exposed_headers    = ["*"]
      max_age_in_seconds = 3600
    }
  }
}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                = local.key_vault_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
}

# Application Insights
resource "azurerm_application_insights" "main" {
  name                = local.app_insights_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
}

# Output important values
output "api_url" {
  value = "https://${azurerm_app_service.api.default_site_hostname}"
}

output "web_url" {
  value = "https://${azurerm_app_service.web.default_site_hostname}"
}

output "sql_server_fqdn" {
  value = azurerm_sql_server.main.fully_qualified_domain_name
}

output "key_vault_uri" {
  value = azurerm_key_vault.main.vault_uri
}
```

## Post-Deployment Steps

1. **Database Initialization**
    - Apply database migrations
    - Seed initial data
    - Verify database schema
2. **Security Configuration**
    - Set up SSL certificates
    - Configure firewall rules
    - Set up network security groups
    - Enable CORS for appropriate origins
3. **User Setup**
    - Create administrator accounts
    - Set up initial roles and permissions
    - Test authentication flow
4. **Integration Testing**
    - Verify API endpoints functionality
    - Test frontend integration with backend
    - Validate external service integrations
    - Test authentication and authorization
5. **Performance Testing**
    - Load testing
    - Stress testing
    - Endurance testing
    - Report generation
6. **Monitoring Setup**
    - Configure alerts
    - Set up dashboards
    - Verify logging
    - Test notification channels