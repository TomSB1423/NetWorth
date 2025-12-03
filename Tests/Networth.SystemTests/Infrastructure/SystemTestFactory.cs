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
        // Helper to write to both test output and console (for CI visibility)
        void Log(string message)
        {
            testOutput?.WriteLine(message);
            Console.WriteLine($"[SystemTestFactory] {message}");
        }

        Log("Initializing SystemTestFactory...");

        IDistributedApplicationTestingBuilder builder = await DistributedApplicationTestingBuilder.CreateAsync<Networth_AppHost>(
            [],
            (appOptions, hostSettings) =>
            {
                hostSettings.EnvironmentName = "Development";
                appOptions.DisableDashboard = !enableDashboard;
                appOptions.AllowUnsecuredTransport = enableDashboard;
            });

        // Load user secrets from AppHost project - DistributedApplicationTestingBuilder doesn't load them by default
        ConfigurationBuilder configBuilder = new();
        configBuilder.AddUserSecrets(typeof(Networth_AppHost).Assembly);
        configBuilder.AddEnvironmentVariables();
        IConfigurationRoot userSecretsConfig = configBuilder.Build();

        // Validate required parameters
        string[] requiredParameters = ["gocardless-secret-id", "gocardless-secret-key"];
        List<string> missingParameters = [];

        foreach (string param in requiredParameters)
        {
            string? value = userSecretsConfig[$"Parameters:{param}"];
            if (string.IsNullOrWhiteSpace(value))
            {
                missingParameters.Add(param);
            }
        }

        if (missingParameters.Count > 0)
        {
            string error = $"Missing required configuration parameters for system tests: {string.Join(", ", missingParameters)}. " +
                           "Ensure these are set in User Secrets (local) or Environment Variables (CI) with 'Parameters__' prefix (e.g. Parameters__gocardless-secret-id).";
            Log(error);
            throw new InvalidOperationException(error);
        }

        // Ensure postgres-password is set (required by AppHost), default to a test value if missing
        if (string.IsNullOrWhiteSpace(userSecretsConfig["Parameters:postgres-password"]))
        {
            Log("Setting default 'postgres-password' for test environment.");
            builder.Configuration["Parameters:postgres-password"] = "test-password-123!";
        }

        // Copy Parameters from user secrets to the test builder's configuration
        var parameters = userSecretsConfig.GetSection("Parameters").GetChildren().ToList();
        Log($"Found {parameters.Count} parameters in 'Parameters' configuration section.");

        foreach (IConfigurationSection section in parameters)
        {
            Log($"Setting Parameter: {section.Key} (Value length: {section.Value?.Length ?? 0})");
            builder.Configuration[$"Parameters:{section.Key}"] = section.Value;
        }

        // Apply standard system test setup
        // Random volume names ensure test isolation from development environment
        builder.WithRandomVolumeNames();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole();
            logging.AddFakeLogging();
            if (testOutput is not null)
            {
                logging.AddXUnit(testOutput);
            }

            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter("Aspire", LogLevel.Debug);
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Debug);
        });

        // Build and start the application
        Log("Building application...");
        DistributedApplication app = await builder.BuildAsync();

        Log("Starting application...");
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(180));
        await app.StartAsync(cts.Token);

        try
        {
            Log("Waiting for Functions resource to be healthy...");
            await app.ResourceNotifications.WaitForResourceHealthyAsync(
                ResourceNames.Functions,
                cts.Token);
            Log("Functions resource is healthy.");
        }
        catch (Exception ex)
        {
            // Capture diagnostic information
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Failed to wait for Functions resource: {ex.Message}");
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine("RESOURCE STATES:");

            var rns = app.Services.GetRequiredService<ResourceNotificationService>();
            var appModel = app.Services.GetRequiredService<DistributedApplicationModel>();

            foreach (IResource resource in appModel.Resources)
            {
                var snapshot = await rns.WaitForResourceAsync(resource.Name, re => true, cts.Token)
                    .ContinueWith(t => t.IsCompletedSuccessfully ? t.Result.Snapshot : null, TaskScheduler.Default);

                var state = snapshot?.State?.Text ?? "unknown";
                var health = snapshot?.HealthStatus?.ToString() ?? "unknown";

                string info = $" - {resource.Name}: State={state}, Health={health}";
                sb.AppendLine(info);
                Log(info); // Also log to console just in case
            }

            sb.AppendLine("--------------------------------------------------");

            // Throw a new exception with the diagnostic info in the message so it appears in the test failure report
            throw new InvalidOperationException(sb.ToString(), ex);
        }

        return app;
    }
}
