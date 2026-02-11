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

    /// <summary>
    ///     Disables non-essential resources (frontend, docs) in the test builder to avoid
    ///     port conflicts and unnecessary resource usage during integration tests.
    ///     Changes fixed ports to null (random) so they don't conflict across test classes.
    /// </summary>
    internal static void DisableNonEssentialResources(IDistributedApplicationTestingBuilder builder)
    {
        HashSet<string> nonEssentialNames = [ResourceNames.React, ResourceNames.Docs, "api-reference"];

        foreach (IResource resource in builder.Resources.Where(r => nonEssentialNames.Contains(r.Name)))
        {
            // Replace fixed port assignments with null (random) to avoid conflicts
            foreach (EndpointAnnotation endpoint in resource.Annotations.OfType<EndpointAnnotation>())
            {
                endpoint.Port = null;
            }
        }
    }

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
                appOptions.AllowUnsecuredTransport = true;
            });

        // Provide default values for all required parameters to avoid resolution failures
        builder.Configuration["Parameters:firebase-api-key"] = "test-api-key";
        builder.Configuration["Parameters:firebase-auth-domain"] = "test.firebaseapp.com";
        builder.Configuration["Parameters:firebase-project-id"] = "test-project";
        builder.Configuration["Parameters:mock-authentication"] = "true";

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

        // Disable non-essential resources to avoid port conflicts in CI.
        // The frontend (port 3000) and docs (port 3001) are NpmApp resources that
        // are not needed for integration tests and cause port binding issues when
        // multiple test classes create separate Aspire apps sequentially.
        DisableNonEssentialResources(builder);

        // Disable real authentication for integration tests (use mock user)
        functionsResource.Annotations.Add(new EnvironmentCallbackAnnotation(env =>
        {
            env.EnvironmentVariables["Networth__UseAuthentication"] = "false";
            env.EnvironmentVariables["Firebase__ProjectId"] = "disabled";
            // Override Frontend__Url since frontend is disabled in tests
            env.EnvironmentVariables["Frontend__Url"] = "http://localhost:3000";
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
