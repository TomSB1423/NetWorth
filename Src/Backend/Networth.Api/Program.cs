using System.Reflection;
using Networth.Extensions;
using Networth.Middleware;
using Scalar.AspNetCore;
using Serilog;

// Create a bootstrap logger for startup logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging
builder.Host.AddSerilogLogging();

// Configure options using extension method
builder.Services.AddOptionsConfiguration(builder.Configuration);

// Add HTTP request logging
builder.Services.AddHttpRequestLogging();

// Add OpenAPI services
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure logging middleware
app.UseLoggingMiddleware();

// Add error handling middleware first to catch all exceptions
app.UseErrorHandling();

app.RegisterFeatureRouters();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .CacheOutput();
    app.MapScalarApiReference();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}
else
{
    // TODO: Always use HTTPS - configure to use for local development as well
    // https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-9.0
    app.UseHttpsRedirection();
}

app.UseRouting();

app.Run();