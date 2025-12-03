# Networth Application - AI Coding Agent Instructions

## Agent Tips

- Use context7 MCP server for obtaining documentation and codebase context.
- Use Playwrite to interact with the browser-based parts of the application if needed.


## Architecture Overview

This is a .NET 9 financial aggregation application using **.NET Aspire** for orchestration. The architecture follows a clean/onion pattern with clear separation of concerns:

- **Networth.AppHost**: .NET Aspire orchestrator - manages all services, PostgreSQL, Azure Storage emulator, and frontend
- **Networth.Functions**: Azure Functions backend (HTTP triggers + Queue triggers)
- **Networth.Application**: Business logic layer with custom mediator pattern (commands/queries/handlers)
- **Networth.Infrastructure**: Data access (EF Core), external integrations (GoCardless API via Refit)
- **Networth.Domain**: Domain entities and repository interfaces
- **Networth.Frontend**: React SPA (Create React App) with custom hooks pattern
- **Networth.ServiceDefaults**: Shared Aspire configuration and resource name constants

**Data Flow**: React → Azure Functions (HTTP) → Mediator → Handler → Repository/External API → PostgreSQL/GoCardless
**Async Flow**: Functions enqueue to Azure Storage Queue → Queue trigger processes sync operations

## Running the Application

**Primary Command** (runs everything):
```bash
dotnet run --project Networth.AppHost
```

This starts:
- PostgreSQL (containerized) with optional PgAdmin on port 5050
- Azure Storage Emulator (Azurite) with queues
- Azure Functions backend on random port (Aspire-managed)
- React frontend on port 3000
- Aspire Dashboard at https://localhost:17065

**Required Secrets** (set before first run):
```bash
dotnet user-secrets set "Gocardless:SecretId" "YOUR_SECRET_ID" --project Networth.AppHost
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_SECRET_KEY" --project Networth.AppHost
```

**API Documentation**: Access Scalar UI at `/api/docs` endpoint on the Functions service (via Aspire dashboard)

## Key Conventions & Patterns

### Custom Mediator Pattern (NOT MediatR)
This project uses a **custom, lightweight mediator** (`Networth.Application.Services.Mediator`), NOT the MediatR library.

**Request/Handler Registration Pattern**:
```csharp
// In ServiceCollectionExtensions.cs:
services.AddScoped<IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>, GetAccountsQueryHandler>();
services.AddScoped<IValidator<GetAccountsQuery>, GetAccountsQueryValidator>();
```

**Usage in Functions**:
```csharp
var result = await mediator.Send<TRequest, TResponse>(request, cancellationToken);
```

**When adding new features**:
1. Create query/command in `Networth.Application/Queries` or `Commands`
2. Create handler implementing `IRequestHandler<TRequest, TResponse>`
3. Optional: Create validator implementing `IValidator<TRequest>` (FluentValidation)
4. Register handler + validator in `Networth.Application/Extensions/ServiceCollectionExtensions.cs`

### Azure Functions Structure
Functions use **isolated worker process** model with ASP.NET Core integration:

```csharp
[Function("GetAccounts")]
[OpenApiOperation("GetAccounts", "Accounts", ...)] // For Scalar/Swagger docs
public async Task<IActionResult> RunAsync(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")] HttpRequest req)
```

**Organization**: `Networth.Functions/Functions/Http/{Resource}/{Action}.cs` (e.g., `Accounts/GetAccounts.cs`)

**Queue Functions**: Use `[QueueTrigger("queue-name", Connection = "queues")]` - see `SyncAccount.cs` for pattern

### Dependency Management
**Centralized Package Management** (CPM) enabled via `Directory.Packages.props`:
- Add `<PackageVersion Include="PackageName" Version="x.y.z" />` to `Directory.Packages.props`
- Reference in `.csproj` files with `<PackageReference Include="PackageName" />` (NO version attribute)

### Code Quality Standards
Configured in `Directory.Build.props`:
- **TreatWarningsAsErrors**: true (build fails on warnings)
- **Nullable reference types**: enabled globally
- **Analyzers**: StyleCop, SerilogAnalyzer, Microsoft.CodeAnalysis.NetAnalyzers, Threading.Analyzers
- **Documentation**: XML comments required for all public APIs

### Database Patterns
**EF Core with PostgreSQL** via Npgsql:
- Context: `Networth.Infrastructure.Data.Context.NetworthDbContext`
- **Database creation**: Automatic in Development/Test environments (`Program.cs` calls `EnsureCreatedAsync`)
- **Note**: Infrastructure entities (`Networth.Infrastructure.Data.Entities`) differ from domain entities (`Networth.Domain.Entities`)
- Domain entities are often **read-only records** from external APIs; infrastructure entities are EF-mapped classes

**Repository Pattern**: Interface in Domain, implementation in Infrastructure (`Networth.Infrastructure.Data.Repositories`)

### External Integrations
**GoCardless API** (bank account data provider):
- Interface: `IGocardlessClient` (Refit-based, configured in `ServiceCollectionExtensions.cs`)
- Custom auth handler: `GoCardlessAuthHandler` with `GoCardlessTokenManager`
- Retry handler: `RefitRetryHandler`
- Service layer: `GocardlessService` implements `IFinancialProvider`
- **JSON naming**: snake_case for GoCardless API (configured via `SystemTextJsonContentSerializer`)

### Frontend Architecture
**React with Custom Hooks**:
- Service layer pattern in `src/services/`
- Custom hooks in `src/hooks/` (e.g., `useInstitutions` for data fetching + state management)
- **Mock data** in `src/constants/mockData.js` for development

**API integration**: Services call Functions backend via environment-configured base URL

## Testing
Integration tests in `Tests/Networth.Functions.Tests.Integration/`:
- Uses **Aspire.Hosting.Testing** for spinning up full app stack
- **Mockoon** for mocking external GoCardless API
- **Testcontainers** for PostgreSQL in tests

Run tests: `dotnet test`

## Common Tasks

**Add a new API endpoint**:
1. Create handler in `Networth.Application/Handlers/`
2. Create query/command in `Networth.Application/Queries` or `Commands`
3. Register in `ServiceCollectionExtensions.cs`
4. Create Function in `Networth.Functions/Functions/Http/{Resource}/`
5. Add `[OpenApiOperation]` attributes for API docs

**Add a queue-triggered background job**:
1. Create command + handler for business logic
2. Create Function with `[QueueTrigger]` in `Networth.Functions/Functions/Queues/`
3. Enqueue messages using `IQueueService` (see `QueueService` implementation)

**Update dependencies**:
- Modify `Directory.Packages.props` only
- Run `dotnet restore` at solution root

**Database changes**:
- Currently using `EnsureCreatedAsync()` (code-first, no migrations yet)
- Modify entity configurations in `Networth.Infrastructure/Data/Configurations/`

## Resource Names
Constants defined in `Networth.ServiceDefaults.ResourceNames`:
- `Postgres`, `NetworthDb`, `Functions`, `React`, `Storage`, `Queues`
- Use these constants consistently across AppHost and service configuration
