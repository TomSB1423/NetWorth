# NetWorth Application - AI Coding Instructions

## Architecture Overview

**NetWorth** is a full-stack personal finance application with a .NET backend and React frontend, orchestrated using .NET Aspire.

### Tech Stack
- **Backend**: Azure Functions (isolated worker), .NET 9.0, Entity Framework Core
- **Frontend**: React 19 with Create React App, Tailwind CSS
- **Orchestration**: .NET Aspire (AppHost for local development)
- **Database**: PostgreSQL (via Aspire hosting), SQLite for development
- **External APIs**: GoCardless Bank Account Data API
- **Testing**: xUnit, Moq, FluentValidation

### Solution Structure
```
Networth.Backend/
  ├── Networth.Backend.Functions/     # Azure Functions HTTP triggers
  ├── Networth.Backend.Application/   # CQRS handlers, mediator, validation
  ├── Networth.Backend.Domain/        # Domain entities, enums, interfaces
  ├── Networth.Backend.Infrastructure/# EF Core, GoCardless API, Refit clients
  └── Tests/                          # Unit tests
Networth.Frontend/networth-frontend-react/ # React SPA
Networth.AppHost/                     # .NET Aspire orchestration
Networth.ServiceDefaults/             # Shared Aspire configuration
```

## Critical Development Workflows

### Running the Application

**Preferred: Use .NET Aspire AppHost** (orchestrates all services)
```bash
# From repo root
dotnet run --project Networth.AppHost
```
This starts:
- Azure Functions backend at `http://localhost:7071`
- React frontend at `http://localhost:3000`
- PostgreSQL database (containerized)

**Alternative: Backend only with Docker Compose**
```bash
cd Networth.Backend
docker compose up --build
```
Services: Backend (port 7071), Azurite storage emulator (ports 10000-10002)

**Frontend standalone**
```bash
cd Networth.Frontend/networth-frontend-react
npm start
```

### User Secrets for GoCardless
Required before running backend:
```bash
dotnet user-secrets set "Gocardless:SecretId" "YOUR_SECRET_ID"
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_SECRET_KEY"
```

### Build & Test Tasks
- **Build backend**: Use VS Code task "build (functions)" or "build (api)"
- **Run tests**: `dotnet test` from solution root
- **Code analysis**: Enabled globally via `Directory.Build.props` (StyleCop, Roslyn analyzers, warnings-as-errors)

## Backend Architecture Patterns

### CQRS with Custom Mediator
The backend uses a **lightweight mediator pattern** without MediatR:

1. **Commands/Queries**: Implement `IRequest<TResponse>` marker interface
2. **Handlers**: Implement `IRequestHandler<TRequest, TResponse>` with `HandleAsync` method
3. **Validators**: FluentValidation validators auto-executed by mediator
4. **Mediator flow**: Validates → Executes handler

**Example: Adding a new query**
```csharp
// 1. Define query in Application/Queries/
public class GetAccountQuery : IRequest<Account>
{
    public string AccountId { get; set; }
}

// 2. Create handler in Application/Handlers/
public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Account>
{
    public async Task<Account> HandleAsync(GetAccountQuery request, CancellationToken ct)
    {
        // Implementation
    }
}

// 3. Register in Application/Extensions/ServiceCollectionExtensions.cs
services.AddScoped<IRequestHandler<GetAccountQuery, Account>, GetAccountQueryHandler>();

// 4. Use in Azure Function
public class GetAccount(IMediator mediator)
{
    [Function("GetAccount")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
        string accountId)
    {
        var result = await mediator.Send<GetAccountQuery, Account>(
            new GetAccountQuery { AccountId = accountId });
        return new OkObjectResult(result);
    }
}
```

### Azure Functions Structure
- **Functions folder**: HTTP-triggered endpoints with OpenAPI attributes
- **DI via Program.cs**: Register services using extension methods (`AddApplicationServices()`, `AddInfrastructure()`)
- **Middleware**: `ExceptionHandlerMiddleware` for global error handling
- **Configuration**: `settings.json`, `local.settings.json`, user secrets, environment variables

### Database & Entity Framework
- **DbContext**: `NetworthDbContext` only stores `Requisitions` and `Accounts` (minimal local storage)
- **Most data**: Fetched from GoCardless API via `IFinancialProvider` (no local caching of transactions/balances)
- **Migrations**: Located in `Infrastructure/Data/Migrations/`
- **Current DB**: SQLite for dev, PostgreSQL configured for production (see `DatabaseOptions`)

### External API Integration (GoCardless)
- **Client**: `IGocardlessClient` using Refit with snake_case JSON serialization
- **Auth**: `GoCardlessAuthHandler` auto-injects bearer tokens via `GoCardlessTokenManager`
- **Retry Logic**: `RefitRetryHandler` for resilience
- **Service**: `GocardlessService` implements `IFinancialProvider` domain interface

## Frontend Architecture Patterns

### Component Organization
```
src/
  ├── components/          # Reusable UI components
  │   ├── common/          # Shared components (buttons, inputs, etc.)
  │   └── institutions/    # Feature-specific components
  ├── pages/               # Top-level page components
  ├── services/            # API service layer
  ├── hooks/               # Custom React hooks
  └── constants/           # Shared constants (COLORS palette)
```

### Styling Conventions
- **Use `COLORS` from `constants/colors.js`** for consistent theming (dark mode palette)
- **Avoid inline styles**: Use CSS modules or Tailwind classes
- **Responsive design**: Mobile-first approach

### React Patterns
- **Functional components** with hooks (no class components)
- **PropTypes** for prop validation (TypeScript migration pending)
- **Component naming**: PascalCase for files and components

## Code Quality & Standards

### .NET Conventions
- **.NET 9.0** target (via `Directory.Build.props`)
- **Central Package Management**: All versions in `Directory.Packages.props`
- **Nullable reference types**: Enabled (`<Nullable>enable</Nullable>`)
- **XML documentation**: Required for public APIs (`GenerateDocumentationFile`)
- **Code analyzers**: StyleCop, SerilogAnalyzer, Roslyn analyzers (warnings-as-errors)
- **Primary constructors**: Preferred for DI (see existing handlers/functions)

### Testing Strategy
- **Location**: `Networth.Backend/Tests/`
- **Framework**: xUnit + Moq + FluentAssertions (if added)
- **Coverage**: Focus on handlers, validators, service logic
- **Write tests** when adding new handlers or complex business logic

## Common Pitfalls & Tips

1. **Don't manually manage handler registration**: Add to `ServiceCollectionExtensions.cs`
2. **Validation is automatic**: Just register validators with `IValidator<TRequest>`
3. **Database context**: Only for Requisitions/Accounts; don't add unnecessary DbSets
4. **GoCardless DTOs**: Use snake_case serialization (configured in `RefitSettings`)
5. **Aspire environment**: Reference services using `builder.AddReference()` for service discovery
6. **Frontend API base URL**: Set via environment variable or Aspire service reference

## Key Files Reference
- `AppHost.cs`: Aspire orchestration (adds Postgres, Functions, React)
- `Program.cs` (Functions): DI setup, middleware, Serilog
- `ServiceCollectionExtensions.cs`: Where to register new services/handlers
- `NetworthDbContext.cs`: EF Core context (minimal entities)
- `COLORS.js`: Frontend color palette - use for consistency
