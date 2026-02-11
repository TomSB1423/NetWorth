using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Networth.Application.Extensions;
using Networth.Functions.Extensions;
using Networth.Functions.Middleware;
using Networth.Functions.Options;
using Networth.Infrastructure.Extensions;
using Networth.ServiceDefaults;
using Serilog;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.AddServiceDefaults();

// FunctionContextMiddleware must be first to capture the context for DI
builder.UseMiddleware<FunctionContextMiddleware>();
builder.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure additional app settings before reading options
builder.Configuration
    .AddJsonFile("settings.json", false, true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Read authentication setting early to decide which auth middleware to use
var networthOptions = new NetworthOptions();
builder.Configuration.GetSection(NetworthOptions.SectionName).Bind(networthOptions);

// Use mock auth in development when UseAuthentication is disabled, otherwise use real JWT auth
if (builder.Environment.IsDevelopment() && !networthOptions.UseAuthentication)
{
    // Mock user middleware injects a default mock user for all requests
    builder.UseMiddleware<MockUserMiddleware>();
}
else
{
    // JWT authentication for production and when testing with real Firebase tokens
    builder.UseWhen<JwtAuthenticationMiddleware>(context =>
    {
        var isHttpTrigger = context.FunctionDefinition.InputBindings.Values
            .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";

        string[] excludedFunctions = ["GetHealth", "RenderSwaggerDocument", "RenderSwaggerUI", "RenderOpenApiDocument"];
        return isHttpTrigger && !excludedFunctions.Contains(context.FunctionDefinition.Name);
    });
}

// Resolve internal user ID after JWT authentication (for endpoints that require an existing user)
builder.UseWhen<UserResolutionMiddleware>(context =>
{
    var isHttpTrigger = context.FunctionDefinition.InputBindings.Values
        .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";

    // Exclude endpoints that handle user resolution themselves or don't require a user:
    // - CreateUser: Creates the user, so they don't exist yet
    // - GetCurrentUser: Returns 404 itself if user not found
    // - GetInstitutions: Works with or without a user (filters already-linked institutions if user exists)
    string[] excludedFunctions =
    [
        "GetHealth",
        "RenderSwaggerDocument",
        "RenderSwaggerUI",
        "RenderOpenApiDocument",
        "CreateUser",
        "GetCurrentUser",
        "GetInstitutions",
    ];
    return isHttpTrigger && !excludedFunctions.Contains(context.FunctionDefinition.Name);
});

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.AllowTrailingCommas = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.PropertyNameCaseInsensitive = true;
    options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// Add Aspire observability to postgres
builder.AddNpgsqlDataSource(ResourceNames.NetworthDb);

// Add Aspire Azure Queue Service client
builder.AddAzureQueueServiceClient(ResourceNames.Queues);

// Configure services
builder.Services
    .AddSerilog(configuration => { configuration.ReadFrom.Configuration(builder.Configuration); })
    .AddApplicationInsightsTelemetryWorkerService()
    .AddAppAuthentication(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

// Register validators from the Functions assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure Firebase options
builder.Services.AddOptions<FirebaseOptions>()
    .Bind(builder.Configuration.GetSection(FirebaseOptions.SectionName))
    .ValidateFluently()
    .ValidateOnStart();

// Configure Networth options (simple boolean flag, no validation needed)
builder.Services.AddOptions<NetworthOptions>()
    .Bind(builder.Configuration.GetSection(NetworthOptions.SectionName));

IHost host = builder.Build();

await host.RunAsync();
