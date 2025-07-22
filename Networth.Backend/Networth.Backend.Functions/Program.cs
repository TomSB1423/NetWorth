using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Networth.Backend.Infrastructure.Extensions;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults()
    .ConfigureFunctionsWebApplication();

// Configure additional app settings
builder.Configuration
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure services
builder.Services.AddApplicationInsightsTelemetryWorkerService();

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();

host.Run();
