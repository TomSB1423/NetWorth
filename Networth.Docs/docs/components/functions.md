---
sidebar_position: 1
---

# Azure Functions Component

The `Networth.Functions` project contains all HTTP and queue-triggered Azure Functions that serve as the API layer for the application.

## Overview

**Technology**: Azure Functions (.NET 9 Isolated Worker)
**Purpose**: Serverless API endpoints and background job processing
**Location**: `/Networth.Functions`

## Project Dependencies

```xml
<PackageReference Include="Microsoft.Azure.Functions.Worker" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.OpenApi" />
<PackageReference Include="Azure.Storage.Queues" />
```

## Function Categories

### HTTP Functions

Located in `Functions/Http/{Resource}/`, these provide RESTful API endpoints.

#### Accounts
- **GetAccounts**: Retrieve all accounts for a user
- **GetAccountBalances**: Get current balances for an account
- **GetAccountDetails**: Get account details (IBAN, currency, etc.)
- **GetAccountTransactions**: Retrieve transaction history
- **LinkAccount**: Link a new financial account

#### Institutions
- **GetInstitutions**: List available financial institutions by country
- **SyncInstitution**: Trigger sync for all accounts at an institution

#### Requisitions
- **GetRequisition**: Check status of account linking process

#### Health
- **Health**: Health check endpoint for monitoring

### Queue Functions

Located in `Functions/Queues/`, these process background jobs.

#### SyncAccount
Processes account synchronization jobs from the `account-sync` queue:

```csharp
[Function("SyncAccount")]
public async Task Run(
    [QueueTrigger("account-sync", Connection = "queues")] string queueMessage)
{
    var message = JsonSerializer.Deserialize<SyncAccountMessage>(queueMessage);
    
    var command = new SyncAccountCommand(
        message.AccountId,
        message.DateFrom,
        message.DateTo
    );

    await _mediator.Send<SyncAccountCommand, Unit>(command, CancellationToken.None);
}
```

## Authorization

Functions use `AuthorizationLevel.Function` which requires:
- Function key in production
- No key needed in local development

```csharp
[HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")]
```

## OpenAPI Documentation

Each HTTP function includes OpenAPI attributes:

```csharp
[Function("GetAccounts")]
[OpenApiOperation(
    operationId: "GetAccounts",
    tags: ["Accounts"],
    Summary = "Get all accounts for a user",
    Description = "Retrieves all linked financial accounts for the specified user"
)]
[OpenApiParameter(
    name: "userId",
    In = ParameterLocation.Query,
    Required = true,
    Type = typeof(Guid),
    Description = "The unique identifier of the user"
)]
[OpenApiResponseWithBody(
    statusCode: HttpStatusCode.OK,
    contentType: "application/json",
    bodyType: typeof(GetAccountsQueryResult),
    Description = "Successfully retrieved accounts"
)]
[OpenApiResponseWithoutBody(
    statusCode: HttpStatusCode.BadRequest,
    Description = "Invalid userId provided"
)]
public async Task<IActionResult> RunAsync(...)
```

This generates OpenAPI specification at `/api/swagger.json`.

## Error Handling

Functions use middleware for consistent error handling:

```csharp
builder.ConfigureFunctionsWebApplication(app =>
{
    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.UseMiddleware<ValidationMiddleware>();
});
```

### Exception Middleware

Catches unhandled exceptions and returns structured errors:

```json
{
  "error": "An error occurred",
  "details": "Account not found",
  "statusCode": 404
}
```

### Validation Middleware

Intercepts requests and validates using FluentValidation before reaching handlers.

## Configuration

Functions access configuration through `IConfiguration`:

```csharp
public class MyFunction
{
    private readonly IConfiguration _configuration;

    public MyFunction(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Access connection strings
        var dbConnection = _configuration.GetConnectionString("NetworthDb");
        var queueConnection = _configuration.GetConnectionString("queues");
        
        // Access app settings
        var secretId = _configuration["Gocardless:SecretId"];
    }
}
```

Configuration sources (in order of precedence):
1. Environment variables (set by Aspire)
2. User secrets (local development)
3. `appsettings.json`

## Dependency Injection

Services are registered in `Program.cs`:

```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Application services
        services.AddApplication();      // Mediator, handlers, validators
        services.AddInfrastructure();   // Repositories, API clients
        
        // Register Functions-specific services
        services.AddScoped<IQueueService, QueueService>();
    })
    .Build();
```

## Testing

Functions are tested using integration tests with Aspire:

```csharp
public class GetAccountsTests : IClassFixture<AspireFixture>
{
    private readonly AspireFixture _fixture;

    public GetAccountsTests(AspireFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAccounts_WithValidUserId_ReturnsAccounts()
    {
        // Arrange
        var client = _fixture.CreateHttpClient("functions");
        var userId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/accounts?userId={userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GetAccountsQueryResult>();
        Assert.NotNull(result);
    }
}
```

## Local Development

Run Functions locally:

```bash
# Via Aspire (recommended)
dotnet run --project Networth.AppHost

# Standalone (not recommended, missing dependencies)
cd Networth.Functions
func start
```

## Monitoring

Functions integrate with Application Insights for:
- Request tracking
- Exception logging
- Performance metrics
- Custom telemetry

```csharp
private readonly TelemetryClient _telemetry;

public MyFunction(TelemetryClient telemetry)
{
    _telemetry = telemetry;
}

public async Task Run(...)
{
    _telemetry.TrackEvent("AccountSynced", new Dictionary<string, string>
    {
        ["AccountId"] = accountId.ToString()
    });
}
```

## Best Practices

1. **Keep Functions Thin**: Delegate to handlers via mediator
2. **Use Async/Await**: All I/O operations should be async
3. **Document with OpenAPI**: Use attributes for API documentation
4. **Validate Input**: Use FluentValidation for request validation
5. **Handle Errors**: Let middleware handle exceptions consistently
6. **Test Integration**: Use Aspire.Hosting.Testing for realistic tests
