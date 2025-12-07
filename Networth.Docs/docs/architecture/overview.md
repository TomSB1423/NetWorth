---
sidebar_position: 1
---

# Architecture Overview

The Networth application is built using a **clean/onion architecture** pattern with .NET 9 and .NET Aspire for orchestration. This architecture ensures clear separation of concerns and maintainability.

## Architectural Layers

### 1. Orchestration Layer (.NET Aspire)
**Project**: `Networth.AppHost`

.NET Aspire serves as the orchestrator for the entire application stack. It manages:
- Service discovery and configuration
- Container orchestration (PostgreSQL, Azurite)
- Environment configuration
- Service dependencies and startup order

### 2. Presentation Layer
**Projects**: `Networth.Frontend`, `Networth.Functions`

- **Frontend**: React SPA built with Vite and Tailwind CSS
- **API Layer**: Azure Functions providing HTTP endpoints and OpenAPI documentation

### 3. Application Layer
**Project**: `Networth.Application`

Contains business logic using a custom mediator pattern:
- **Commands**: Write operations (LinkAccount, SyncInstitution)
- **Queries**: Read operations (GetAccounts, GetInstitutions)
- **Handlers**: Process commands and queries
- **Validators**: FluentValidation for request validation

### 4. Infrastructure Layer
**Project**: `Networth.Infrastructure`

Implements data access and external integrations:
- **EF Core**: Database access with repository pattern
- **GoCardless Client**: Refit-based API client for bank data
- **Queue Service**: Azure Storage Queue integration
- **Configuration**: Database configurations and mappings

### 5. Domain Layer
**Project**: `Networth.Domain`

Core business entities and contracts:
- Domain entities (Account, Transaction, Balance, etc.)
- Repository interfaces
- Domain services interfaces

### 6. Service Defaults
**Project**: `Networth.ServiceDefaults`

Shared configuration and constants:
- Resource name constants
- Common Aspire configurations
- Service extension methods

## Design Patterns

### Custom Mediator Pattern
The application uses a **custom, lightweight mediator** (not MediatR) for request handling:

```csharp
// Request/Response
public record GetAccountsQuery(Guid UserId) : IRequest<GetAccountsQueryResult>;

// Handler
public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>
{
    public async Task<GetAccountsQueryResult> Handle(
        GetAccountsQuery request, 
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// Registration
services.AddScoped<IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>, 
                   GetAccountsQueryHandler>();
```

### Repository Pattern
Data access is abstracted through repository interfaces:
- Interfaces defined in `Networth.Domain`
- Implementations in `Networth.Infrastructure`
- EF Core for database operations

### Dependency Injection
All components use constructor injection for dependencies, configured in `ServiceCollectionExtensions`.

## Key Architectural Decisions

### 1. Isolated Worker Process for Azure Functions
Uses the **isolated worker process** model for better:
- Dependency injection support
- ASP.NET Core integration
- Testing capabilities

### 2. Separate Domain and Infrastructure Entities
- **Domain entities**: Read-only records from external APIs
- **Infrastructure entities**: EF Core mapped classes for persistence

### 3. Queue-Based Background Processing
Account synchronization happens asynchronously:
1. HTTP endpoint enqueues sync job
2. Queue trigger processes job in background
3. Reduces response time for users

## Technology Choices

| Component | Technology | Rationale |
|-----------|-----------|-----------|
| Orchestration | .NET Aspire | Simplified local development, service discovery |
| Backend | Azure Functions | Serverless, auto-scaling, cost-effective |
| Frontend | React + Vite | Fast development, modern tooling |
| Database | PostgreSQL | Robust, open-source, excellent .NET support |
| ORM | Entity Framework Core | Type-safe, migrations, LINQ support |
| API Client | Refit | Type-safe HTTP client, easy to use |
| Validation | FluentValidation | Expressive, testable validation |

## Next Steps

- [.NET Aspire Setup](aspire) - Learn about the orchestration layer
- [Backend Architecture](backend) - Understand the Azure Functions backend
- [Frontend Architecture](frontend) - Explore the React application
- [Data Flow](data-flow) - See how data moves through the system
