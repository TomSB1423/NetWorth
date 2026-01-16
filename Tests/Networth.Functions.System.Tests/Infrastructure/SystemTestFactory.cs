using Aspire.Hosting.ApplicationModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.ServiceDefaults;
using Projects;
using Xunit.Abstractions;

namespace Networth.SystemTests.Infrastructure;

/// <summary>
///     Factory for creating configured distributed applications for system testing.
///     System tests use the real GoCardless API (not mocks) with proper database migrations.
/// </summary>
public static class SystemTestFactory
{
    /// <summary>
    ///     Creates and starts a distributed application configured for system testing.
    ///     Uses real GoCardless credentials from user secrets with production-like configuration.
    /// </summary>
    /// <param name="testOutput">Optional test output helper for capturing logs.</param>
    /// <param name="enableDashboard">Whether to enable the Aspire dashboard.</param>
    /// <returns>Started DistributedApplication instance.</returns>
    public static async Task<DistributedApplication> CreateAsync(
        ITestOutputHelper? testOutput,
        bool enableDashboard = false)
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Networth_AppHost>(
            [],
            (appOptions, hostSettings) =>
            {
                hostSettings.EnvironmentName = "Development";
                appOptions.DisableDashboard = !enableDashboard;
                appOptions.AllowUnsecuredTransport = enableDashboard;
            });

        // Ensure user secrets are loaded from the AppHost assembly
        builder.Configuration.AddUserSecrets<Networth_AppHost>();

        // Provide default values for Firebase parameters to avoid resolution failures
        // These will be overridden by user secrets if available
        if (string.IsNullOrEmpty(builder.Configuration["Parameters:firebase-api-key"]))
        {
            builder.Configuration["Parameters:firebase-api-key"] = "test-api-key";
        }

        if (string.IsNullOrEmpty(builder.Configuration["Parameters:firebase-auth-domain"]))
        {
            builder.Configuration["Parameters:firebase-auth-domain"] = "test.firebaseapp.com";
        }

        if (string.IsNullOrEmpty(builder.Configuration["Parameters:firebase-project-id"]))
        {
            builder.Configuration["Parameters:firebase-project-id"] = "test-project";
        }

        builder.Configuration["Parameters:mock-authentication"] = "true";

        // Override the environment variables on the Functions resource directly
        // This bypasses the parameter resolution which seems to fail in test context
        var functionsResource = builder.Resources.FirstOrDefault(r => r.Name == ResourceNames.Functions);
        functionsResource?.Annotations.Add(new EnvironmentCallbackAnnotation(context =>
        {
            context.EnvironmentVariables["Networth__MockAuthentication"] = "true";
            // Disable sandbox mode for system tests - use real Institutions table with API sync
            context.EnvironmentVariables["Institutions__UseSandbox"] = "false";
        }));

        // Apply standard system test setup
        // Random volume names ensure test isolation from development environment
        builder.WithRandomVolumeNames();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddFakeLogging();
            if (testOutput is not null)
            {
                logging.AddXUnit(testOutput);
            }

            logging.SetMinimumLevel(LogLevel.Information);
            logging.AddFilter("Aspire", LogLevel.Warning);
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Information);
        });

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        // Build and start the application
        DistributedApplication app = await builder.BuildAsync();

        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(180));
        await app.StartAsync(cts.Token);

        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            ResourceNames.Functions,
            cts.Token);

        return app;
    }

    /// <summary>
    ///     Gets the PostgreSQL database connection string from the running application.
    /// </summary>
    public static async Task<string> GetDatabaseConnectionStringAsync(DistributedApplication app)
    {
        // Get the connection string from the networth-db resource
        var connectionString = await app.GetConnectionStringAsync(ResourceNames.NetworthDb);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string not found");
        }

        return connectionString;
    }

    /// <summary>
    ///     Creates a DbContext for direct database access in tests.
    /// </summary>
    public static NetworthDbContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NetworthDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new NetworthDbContext(optionsBuilder.Options);
    }
}
