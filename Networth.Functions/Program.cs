using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Networth.Application.Extensions;
using Networth.Functions.Authentication;
using Networth.Functions.Middleware;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Extensions;
using Networth.ServiceDefaults;
using Serilog;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.AddServiceDefaults();

// Middleware
builder.UseMiddleware<MockAuthenticationMiddleware>();
builder.UseMiddleware<ExceptionHandlerMiddleware>();

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
    .AddScoped<ICurrentUserService, CurrentUserService>()
    .AddApplicationServices()
    .AddInfrastructure(builder.Configuration);

IHost host = builder.Build();

IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();
if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
{
    await using AsyncServiceScope serviceScope = host.Services.CreateAsyncScope();
    await using NetworthDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<NetworthDbContext>();

    // In development, create the database if it doesn't exist
    await dbContext.Database.EnsureCreatedAsync();

    // Create required queues
    QueueServiceClient queueServiceClient = serviceScope.ServiceProvider.GetRequiredService<QueueServiceClient>();
    QueueClient accountSyncQueue = queueServiceClient.GetQueueClient(ResourceNames.AccountSyncQueue);
    await accountSyncQueue.CreateIfNotExistsAsync();
}

host.Run();
