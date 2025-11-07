using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Networth.Backend.Application.Extensions;
using Networth.Backend.Functions.Authentication;
using Networth.Backend.Functions.Middleware;
using Networth.Backend.Infrastructure.Data.Seeders;
using Networth.Backend.Infrastructure.Extensions;
using Serilog;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

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
builder.AddNpgsqlDataSource("networth-db");

// Configure services
builder.Services
    .AddSerilog(configuration => { configuration.ReadFrom.Configuration(builder.Configuration); })
    .AddApplicationInsightsTelemetryWorkerService()
    .AddScoped<ICurrentUserService, CurrentUserService>()
    .AddApplicationServices()
    .AddInfrastructure(builder.Configuration);

IHost host = builder.Build();

// Seed mock user for development
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
host.SeedMockUserAsync().GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

host.Run();
