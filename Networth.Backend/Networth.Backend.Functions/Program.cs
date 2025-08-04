using FluentValidation;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Networth.Backend.Infrastructure.Extensions;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Configure additional app settings
builder.Configuration
    .AddJsonFile("settings.json", false, true)
    .AddJsonFile("local.settings.json", true, true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Configure services
builder.Services.AddApplicationInsightsTelemetryWorkerService();

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

IHost host = builder.Build();

host.Run();
