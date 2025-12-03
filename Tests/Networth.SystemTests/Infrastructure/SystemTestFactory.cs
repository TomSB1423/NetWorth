using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Networth.Functions.Tests.Integration.Infrastructure;
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
        IDistributedApplicationTestingBuilder builder = await DistributedApplicationTestingBuilder.CreateAsync<Networth_AppHost>(
            [],
            (appOptions, hostSettings) =>
            {
                hostSettings.EnvironmentName = "Development";
                appOptions.DisableDashboard = !enableDashboard;
                appOptions.AllowUnsecuredTransport = enableDashboard;
            });

        // Load user secrets from AppHost project
        ConfigurationBuilder configBuilder = new();
        configBuilder.AddUserSecrets(typeof(Networth_AppHost).Assembly);
        configBuilder.AddEnvironmentVariables();
        IConfigurationRoot userSecretsConfig = configBuilder.Build();

        // Validate required parameters
        string[] requiredParameters = ["gocardless-secret-id", "gocardless-secret-key"];
        List<string> missingParameters = [];

        foreach (string param in requiredParameters)
        {
            if (string.IsNullOrWhiteSpace(userSecretsConfig[$"Parameters:{param}"]))
            {
                missingParameters.Add(param);
            }
        }

        if (missingParameters.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing required configuration parameters for system tests: {string.Join(", ", missingParameters)}. " +
                "Ensure these are set in User Secrets (local) or Environment Variables (CI) with 'Parameters__' prefix.");
        }

        // Ensure postgres-password is set (required by AppHost), default to a test value if missing
        if (string.IsNullOrWhiteSpace(userSecretsConfig["Parameters:postgres-password"]))
        {
            builder.Configuration["Parameters:postgres-password"] = "test-password-123!";
        }

        // Copy Parameters from user secrets to the test builder's configuration
        var parameters = userSecretsConfig.GetSection("Parameters").GetChildren().ToList();

        foreach (IConfigurationSection section in parameters)
        {
            builder.Configuration[$"Parameters:{section.Key}"] = section.Value;
        }

        // Apply standard system test setup
        // Random volume names ensure test isolation from development environment
        builder.WithRandomVolumeNames();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            if (testOutput is not null)
            {
                logging.AddXUnit(testOutput);
            }

            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter("Aspire", LogLevel.Debug);
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Debug);
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
}
