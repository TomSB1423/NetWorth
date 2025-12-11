using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Networth.Application.Extensions;
using Networth.Functions.Extensions;
using Networth.Functions.Middleware;
using Networth.Infrastructure.Extensions;
using Networth.ServiceDefaults;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Configure additional app settings
builder.Configuration
    .AddJsonFile("settings.json", false, true)
    .AddJsonFile("local.settings.json", true, true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.AddServiceDefaults();

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
    .AddFunctionsServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

// Middleware
builder.ConfigureMiddleware();

IHost host = builder.Build();

IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();
if (environment.IsDevelopment())
{
    await host.Services.EnsureDatabaseSetupAsync();
    await host.Services.EnsureQueuesCreatedAsync();
}

await host.RunAsync();
