---
sidebar_position: 3
---

# Backend Architecture

The backend is built with **Azure Functions** using the .NET 9 Isolated Worker model, providing a serverless, scalable API layer.

## Project Structure

```
Networth.Functions/
├── Functions/
│   ├── Http/           # HTTP-triggered functions
│   │   ├── Accounts/
│   │   ├── Institutions/
│   │   ├── Requisitions/
│   │   └── Health/
│   └── Queues/         # Queue-triggered functions
│       └── SyncAccount.cs
├── Extensions/
└── Program.cs
```

## Isolated Worker Model

The application uses the **Azure Functions Isolated Worker Process**:

```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Register application services
        services.AddApplication();
        services.AddInfrastructure();
    })
    .Build();

await host.RunAsync();
```

### Benefits
- Full dependency injection support
- ASP.NET Core integration
- Middleware pipeline
- Better testing capabilities
- Separate process from host

## HTTP Functions

HTTP functions provide RESTful API endpoints with OpenAPI documentation.

### Example: GetAccounts

```csharp
[Function("GetAccounts")]
[OpenApiOperation(
    operationId: "GetAccounts", 
    tags: ["Accounts"],
    Summary = "Get all accounts for a user",
    Description = "Retrieves all linked financial accounts for the specified user")]
[OpenApiParameter(
    name: "userId", 
    In = ParameterLocation.Query,
    Required = true,
    Type = typeof(Guid))]
[OpenApiResponseWithBody(
    statusCode: HttpStatusCode.OK,
    contentType: "application/json",
    bodyType: typeof(GetAccountsQueryResult))]
public async Task<IActionResult> RunAsync(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")] 
    HttpRequest req)
{
    if (!Guid.TryParse(req.Query["userId"], out var userId))
    {
        return new BadRequestObjectResult("Invalid userId");
    }

    var query = new GetAccountsQuery(userId);
    var result = await _mediator.Send<GetAccountsQuery, GetAccountsQueryResult>(
        query, 
        req.HttpContext.RequestAborted);

    return new OkObjectResult(result);
}
```

### Available Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/institutions` | GET | List financial institutions |
| `/api/accounts/link` | POST | Link a new account |
| `/api/requisitions/{id}` | GET | Get requisition status |
| `/api/accounts` | GET | Get user's accounts |
| `/api/accounts/{id}/balances` | GET | Get account balances |
| `/api/accounts/{id}/details` | GET | Get account details |
| `/api/accounts/{id}/transactions` | GET | Get transactions |
| `/api/institutions/{id}/sync` | POST | Trigger account sync |
| `/api/health` | GET | Health check |

## Queue Functions

Queue functions process background jobs triggered by messages in Azure Storage Queues.

### Example: SyncAccount

```csharp
[Function("SyncAccount")]
public async Task Run(
    [QueueTrigger("account-sync", Connection = "queues")] 
    string queueMessage)
{
    var message = JsonSerializer.Deserialize<SyncAccountMessage>(queueMessage);
    
    var command = new SyncAccountCommand(
        message.AccountId,
        message.DateFrom,
        message.DateTo
    );

    await _mediator.Send<SyncAccountCommand, Unit>(
        command, 
        CancellationToken.None);
}
```

### Queue Processing Flow

1. **Enqueue**: HTTP function adds message to queue
2. **Trigger**: Queue function picks up message
3. **Process**: Fetch data from GoCardless API
4. **Store**: Save balances, details, transactions to database

## Mediator Pattern

All business logic is handled through the custom mediator:

```csharp
// In HTTP Function
var result = await _mediator.Send<TRequest, TResponse>(request, cancellationToken);

// Handler
public class GetAccountsQueryHandler 
    : IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountsQueryResult> Handle(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository
            .GetAccountsByUserIdAsync(request.UserId, cancellationToken);
        
        return new GetAccountsQueryResult(accounts);
    }
}
```

## OpenAPI Documentation

Functions expose OpenAPI specification at `/api/swagger.json`:

### Attributes Used
- `[OpenApiOperation]`: Describes the operation
- `[OpenApiParameter]`: Defines parameters
- `[OpenApiRequestBody]`: Describes request body
- `[OpenApiResponseWithBody]`: Defines response

### Scalar Integration

The OpenAPI spec is consumed by Scalar for interactive documentation:

```csharp
// In AppHost
scalar.WithApiReference(functions, options =>
{
    options
        .AddDocument("v1", "Networth API")
        .WithOpenApiRoutePattern("/api/swagger.json")
        .AddServer("/api", "Azure Functions API");
});
```

## Dependency Injection

Services are registered in `ServiceCollectionExtensions`:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        
        // Register all handlers
        services.AddScoped<
            IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>,
            GetAccountsQueryHandler>();
        
        // Register validators
        services.AddScoped<
            IValidator<GetAccountsQuery>,
            GetAccountsQueryValidator>();
        
        return services;
    }
}
```

## Error Handling

Functions use ASP.NET Core middleware for error handling:

```csharp
builder.ConfigureFunctionsWebApplication(app =>
{
    app.UseExceptionHandler();
    app.UseMiddleware<ValidationMiddleware>();
});
```

## Health Checks

Health check endpoint for Aspire monitoring:

```csharp
[Function("Health")]
public IActionResult Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] 
    HttpRequest req)
{
    return new OkObjectResult(new { status = "healthy" });
}
```

## Configuration

Functions access configuration through dependency injection:

```csharp
public class MyHandler
{
    private readonly IConfiguration _configuration;

    public MyHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = _configuration.GetConnectionString("NetworthDb");
    }
}
```

## Testing

Functions are tested using:
- **Aspire.Hosting.Testing**: Spin up full stack
- **xUnit**: Test framework
- **Testcontainers**: PostgreSQL in tests
- **Mockoon**: Mock GoCardless API

```csharp
public class FunctionsTests : IClassFixture<AspireFixture>
{
    [Fact]
    public async Task GetAccounts_ReturnsAccounts()
    {
        // Arrange
        var client = _fixture.CreateHttpClient("functions");
        
        // Act
        var response = await client.GetAsync("/api/accounts?userId=...");
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
}
```
