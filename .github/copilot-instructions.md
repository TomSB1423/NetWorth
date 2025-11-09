---
description: NetWorth - Streamlined AI agent guide
---

# NetWorth Development Guide

## Quick Start
```bash
dotnet run --project Networth.AppHost  # Starts everything (Functions, React, Postgres, Storage)
dotnet user-secrets set "Gocardless:SecretId" "YOUR_ID" --project Networth.Functions
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_KEY" --project Networth.Functions
```

## Architecture Essentials

### Custom CQRS (No MediatR)
- Pattern: `IRequest<T>` → `IRequestHandler<T,R>` with `HandleAsync()`
- FluentValidation auto-runs before handler execution
- Register handlers + validators in `Application/Extensions/ServiceCollectionExtensions.cs`

### Database Strategy
- DbContext has: `User`, `Institution`, `Account`, `Transaction`, `Requisition`
- Transactions/balances fetched from GoCardless API via `IFinancialProvider` (not stored locally)
- PostgreSQL is **ephemeral** - no volume, wipes on Aspire restart (by design)

### Aspire Integration
- **NEVER** manually set `Functions:Worker:HostEndpoint` or `WorkerId` (Aspire injects automatically)
- Auto-provisions Azure Storage for Functions runtime
- Uses ONLY `NpgsqlDataSource` at runtime (no `appsettings.json` connection string fallback)

### Azure Functions Patterns
- **Middleware order critical**: `MockAuthenticationMiddleware` BEFORE `ExceptionHandlerMiddleware` in `Program.cs`
- Mock auth injects user ID `"mock-user-123"` into `FunctionContext.Items["User"]`
- **NO try-catch in Functions** - `ExceptionHandlerMiddleware` handles all exceptions globally
- Functions only handle expected business logic (validation, authorization checks)
- Primary constructors for DI, OpenAPI attributes required

### GoCardless Integration
- Refit client with snake_case JSON serialization
- `GoCardlessAuthHandler` auto-injects bearer tokens
- `RefitRetryHandler` for resilience

## Critical Workflows

### Database Migrations
```bash
# Recommended: Use automated script (handles dynamic credentials)
./scripts/run-migrations.sh              # Apply pending
./scripts/run-migrations.sh add <Name>   # Create + apply new

# Manual (if script unavailable)
docker ps --format "{{.Names}} {{.Ports}}" | grep postgres
docker exec <container> printenv POSTGRES_PASSWORD
cd Networth.Infrastructure
env "ConnectionStrings__networth-db=Host=localhost;Port=<port>;Database=networth-db;Username=postgres;Password=<pwd>" \
  dotnet ef migrations add <Name> --output-dir Data/Migrations
```

### Adding CQRS Query/Command
```csharp
// 1. Create in Application/Queries/ or Application/Commands/
public class GetAccountQuery : IRequest<Account> { public string AccountId { get; set; } }

// 2. Create handler in Application/Handlers/
public class GetAccountQueryHandler(IFinancialProvider provider)
    : IRequestHandler<GetAccountQuery, Account>
{
    public async Task<Account> HandleAsync(GetAccountQuery request, CancellationToken ct) { /* ... */ }
}

// 3. Register BOTH handler and validator in Application/Extensions/ServiceCollectionExtensions.cs
services.AddScoped<IRequestHandler<GetAccountQuery, Account>, GetAccountQueryHandler>();
services.AddScoped<IValidator<GetAccountQuery>, GetAccountQueryValidator>(); // if validator exists

// 4. Use in Function with OpenAPI attributes
public class GetAccount(IMediator mediator) {
    [Function("GetAccount")]
    [OpenApiOperation("GetAccount", "Accounts")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Account))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}")] HttpRequest req,
        string accountId)
    {
        var result = await mediator.Send<GetAccountQuery, Account>(new GetAccountQuery { AccountId = accountId });
        return new OkObjectResult(result);
    }
}
```

## Critical Gotchas

1. **Wrong Postgres package**: Use `Aspire.Hosting.PostgreSQL` NOT `Aspire.Hosting.Azure.PostgreSQL`
2. **Manual Aspire config**: Don't set `Functions:Worker:HostEndpoint` - Aspire manages it
3. **Database persistence**: Data lost on Aspire restart (ephemeral by design)
4. **Handler registration**: Must register in `ServiceCollectionExtensions.cs` (both Application and Infrastructure layers have separate files)
5. **GoCardless JSON**: Uses snake_case (Refit configured for this)
6. **React colors**: Use `COLORS` from `constants/colors.js` (not inline styles)
7. **Middleware order**: Mock auth MUST be before exception handler
8. **No try-catch**: Let `ExceptionHandlerMiddleware` handle exceptions in Functions

## Code Conventions

- Primary constructors for DI
- Central Package Management (`Directory.Packages.props`)
- Warnings-as-errors enabled (`Directory.Build.props`)
- XML docs required for public APIs
- React: Functional components only, PropTypes validation

## Key Files
- `AppHost.cs` - Aspire orchestration
- `Program.cs` (Functions) - Middleware registration order
- `ServiceCollectionExtensions.cs` (Application & Infrastructure) - Handler/service registration
- `NetworthDbContext.cs` - EF Core entities
- `GocardlessService.cs` - Implements `IFinancialProvider`
- `Mediator.cs` - Custom CQRS mediator
