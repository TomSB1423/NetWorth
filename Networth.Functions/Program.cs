using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Networth.Application.Extensions;
using Networth.Functions.Extensions;
using Networth.Functions.Middleware;
using Networth.Infrastructure.Extensions;
using Networth.ServiceDefaults;
using Serilog;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.AddServiceDefaults();

// FunctionContextMiddleware must be first to capture the context for DI
builder.UseMiddleware<FunctionContextMiddleware>();
builder.UseMiddleware<ExceptionHandlerMiddleware>();

// Use conditional middleware to enforce authentication on all HTTP endpoints except Health
builder.UseWhen<JwtAuthenticationMiddleware>(context =>
{
    // We want to use this middleware only for http trigger invocations.
    var isHttpTrigger = context.FunctionDefinition.InputBindings.Values
        .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";

    // Exclude the Health endpoint
    return isHttpTrigger && context.FunctionDefinition.Name != "GetHealth";
});

// Configure additional app settings
builder.Configuration
    .AddJsonFile("settings.json", false, true)
    .AddJsonFile("local.settings.json", true, true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

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
    .AddAppAuthentication(builder.Configuration, builder.Environment)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

IHost host = builder.Build();

// In development, apply migrations and seed mock user automatically
IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();
if (environment.IsDevelopment())
{
    await host.Services.ApplyMigrationsAsync();
    await host.Services.EnsureMockUserAsync();
}

await host.RunAsync();
