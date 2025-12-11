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
///     System tests use real external services (GoCardless sandbox API) rather than mocks.
/// </summary>
public static class SystemTestFactory
{
    /// <summary>
    ///     Creates and starts a distributed application configured for system testing.
    ///     Uses GoCardless sandbox credentials from user secrets.
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
    ///     Ensures the database is created and ready for testing.
    ///     Note: Since we use random volume names, we start with a fresh DB for each test,
    ///     so we don't need to delete/recreate it.
    /// </summary>
    public static async Task ResetDatabaseAsync(string connectionString)
    {
        await using var dbContext = CreateDbContext(connectionString);

        await dbContext.Database.EnsureCreatedAsync();
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
