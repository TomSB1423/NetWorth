using Microsoft.Extensions.Logging;
using Networth.Infrastructure.Gocardless.Options;
using Networth.ServiceDefaults;
using Projects;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Factory for creating configured distributed applications for integration testing.
/// </summary>
public static class DistributedApplicationTestFactory
{
    /// <summary>
    /// Creates and starts a distributed application configured for integration testing with Mockoon.
    /// Includes Session container lifetime, isolated volumes, and GoCardless configuration.
    /// </summary>
    /// <param name="testOutput">Optional test output helper for capturing logs.</param>
    /// <param name="mockoonBaseUrl">Base URL of the Mockoon container.</param>
    /// <param name="enableDashboard">Whether to enable the Aspire dashboard.</param>
    /// <param name="disableFunctions">Whether to disable certain Azure Functions. Defaults to false.</param>
    /// <returns>Started DistributedApplication instance.</returns>
    public static Task<DistributedApplication> CreateAsync(
        ITestOutputHelper? testOutput,
        string mockoonBaseUrl,
        bool enableDashboard = false,
        bool disableFunctions = false)
        => CreateAsync(testOutput, mockoonBaseUrl, true, enableDashboard, disableFunctions);

    private static async Task<DistributedApplication> CreateAsync(
        ITestOutputHelper? testOutput,
        string? mockoonBaseUrl,
        bool configureMockoon,
        bool enableDashboard,
        bool disableFunctions)
    {
        IDistributedApplicationTestingBuilder builder = await DistributedApplicationTestingBuilder.CreateAsync<Networth_AppHost>(
            [],
            (appOptions, hostSettings) =>
            {
                hostSettings.EnvironmentName = "Development";
                appOptions.DisableDashboard = !enableDashboard;
                appOptions.AllowUnsecuredTransport = enableDashboard;
            });

        // Apply standard integration test setup
        // Random volume names ensure each test run gets fresh database state
        builder.WithRandomVolumeNames();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
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

        ProjectResource functionsResource = builder.Resources
            .OfType<ProjectResource>()
            .First(r => r.Name == ResourceNames.Functions);

        // Force enable mock authentication for integration tests
        functionsResource.Annotations.Add(new EnvironmentCallbackAnnotation(env =>
        {
            env.EnvironmentVariables["Networth__MockAuthentication"] = "true";
            env.EnvironmentVariables["Firebase__ProjectId"] = "disabled";
        }));

        // Configure Mockoon for Functions resource if requested
        if (configureMockoon && mockoonBaseUrl is not null)
        {
            functionsResource.Annotations.Add(new EnvironmentCallbackAnnotation(env =>
            {
                env.EnvironmentVariables[$"Gocardless:{nameof(GocardlessOptions.BankAccountDataBaseUrl)}"] = $"{mockoonBaseUrl}/api/v2";
                env.EnvironmentVariables[$"Gocardless:{nameof(GocardlessOptions.SecretId)}"] = "test-secret-id";
                env.EnvironmentVariables[$"Gocardless:{nameof(GocardlessOptions.SecretKey)}"] = "test-secret-key";

                // Disable queue triggers to allow manual verification in tests
                var queueFunctionToggle = disableFunctions ? "true" : "false";
                env.EnvironmentVariables["AzureWebJobs.CalculateRunningBalance.Disabled"] = queueFunctionToggle;
                env.EnvironmentVariables["AzureWebJobs.SyncInstitutionQueue.Disabled"] = queueFunctionToggle;
                env.EnvironmentVariables["AzureWebJobs.SyncAccount.Disabled"] = queueFunctionToggle;
            }));
        }

        // Build and start
        DistributedApplication app = await builder.BuildAsync();
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(180));
        await app.StartAsync(cts.Token);
        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            ResourceNames.Functions,
            cts.Token);

        return app;
    }
}
