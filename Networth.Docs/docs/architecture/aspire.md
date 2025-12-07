---
sidebar_position: 2
---

# .NET Aspire Orchestration

.NET Aspire is the orchestration framework that manages all services in the Networth application, providing a unified development experience.

## What is .NET Aspire?

.NET Aspire is an opinionated stack for building cloud-native applications with .NET. It provides:
- Service discovery and configuration
- Container orchestration
- Telemetry and observability
- Development dashboard

## AppHost Configuration

The `Networth.AppHost` project configures all application services:

```csharp
// PostgreSQL Database
IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres(ResourceNames.Postgres)
    .WithPassword(postgresPassword)
    .WithDataVolume();

IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres
    .AddDatabase(ResourceNames.NetworthDb);

// Azure Storage for queues
IResourceBuilder<AzureStorageResource> storage = builder
    .AddAzureStorage(ResourceNames.Storage)
    .RunAsEmulator();

IResourceBuilder<AzureQueueStorageResource> queues = storage.AddQueues(ResourceNames.Queues);

// Azure Functions Backend
IResourceBuilder<AzureFunctionsProjectResource> functions = builder
    .AddAzureFunctionsProject<Networth_Functions>(ResourceNames.Functions)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(queues)
    .WaitFor(queues)
    .WithEnvironment("Gocardless__SecretId", gocardlessSecretId)
    .WithEnvironment("Gocardless__SecretKey", gocardlessSecretKey)
    .WithHttpHealthCheck("/api/health");

// React Frontend
var frontend = builder.AddNpmApp(ResourceNames.React, "../Networth.Frontend", "dev")
    .WithReference(functions)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("VITE_API_URL", functions.GetEndpoint("http"))
    .WithHttpEndpoint(env: "PORT", port: 3000)
    .WithExternalHttpEndpoints();
```

## Managed Services

### PostgreSQL Database
- **Container**: PostgreSQL official image
- **Port**: Dynamic (Aspire-managed)
- **PgAdmin**: Available on port 5050 (dev only)
- **Volume**: Persistent data storage

### Azure Storage Emulator (Azurite)
- **Purpose**: Local queue storage for background jobs
- **Container**: Azurite
- **Queues**: account-sync queue for processing

### Azure Functions
- **Runtime**: .NET 9 Isolated Worker
- **Port**: Dynamic (Aspire-managed)
- **Health Check**: `/api/health` endpoint
- **Dependencies**: PostgreSQL, Queues

### React Frontend
- **Build Tool**: Vite
- **Port**: 3000
- **Environment**: Configured with Functions endpoint
- **Dependencies**: Azure Functions

### Scalar API Reference
- **Purpose**: Interactive API documentation
- **Integration**: OpenAPI from Functions
- **Theme**: Purple
- **Features**: Auto-discovery of Functions endpoints

## Service Discovery

Aspire provides automatic service discovery:

```csharp
// In Functions
builder.AddServiceDefaults();  // Registers service discovery

// Configuration is injected automatically
var dbConnection = builder.Configuration.GetConnectionString("NetworthDb");
var queueConnection = builder.Configuration.GetConnectionString("queues");
```

## Resource Names

Constants are defined in `Networth.ServiceDefaults.ResourceNames`:

```csharp
public static class ResourceNames
{
    public const string Postgres = "postgres";
    public const string NetworthDb = "networth-db";
    public const string Functions = "functions";
    public const string React = "react";
    public const string Storage = "storage";
    public const string Queues = "queues";
}
```

## Configuration Management

### User Secrets
Sensitive configuration is stored using .NET user secrets:

```bash
dotnet user-secrets set "Gocardless:SecretId" "YOUR_SECRET_ID" --project Networth.AppHost
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_SECRET_KEY" --project Networth.AppHost
```

### Parameters
Configuration parameters are defined in the AppHost:

```csharp
IResourceBuilder<ParameterResource> postgresPassword = 
    builder.AddParameter("postgres-password", secret: true);

IResourceBuilder<ParameterResource> gocardlessSecretId = 
    builder.AddParameter("gocardless-secret-id", secret: true);
```

## Development Dashboard

When running the application, Aspire provides a dashboard at:
```
https://localhost:17065
```

The dashboard shows:
- All running services and their status
- Logs and telemetry
- Resource usage
- Service endpoints
- Distributed tracing

## Running the Application

Start all services with a single command:

```bash
dotnet run --project Networth.AppHost
```

This starts:
1. PostgreSQL container
2. Azurite storage emulator
3. Azure Functions backend
4. React frontend
5. Aspire Dashboard

## Benefits of Aspire

### Local Development
- Single command to start entire stack
- Automatic service discovery
- Consistent environment configuration
- Real-time logs and telemetry

### Production Ready
- Configuration patterns work in production
- Service discovery translates to cloud services
- Telemetry built-in
- Health checks configured

### Testing
- Integration tests can use the same orchestration
- Testcontainers integration
- Mockoon for external API mocking
