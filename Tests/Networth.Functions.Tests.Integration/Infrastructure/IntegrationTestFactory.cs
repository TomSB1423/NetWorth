using Aspire.Hosting;
using Aspire.Hosting.Lifecycle;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Infrastructure;

/// <summary>
/// Factory for creating configured integration test applications.
/// </summary>
public static class IntegrationTestFactory
{
    /// <summary>
    /// Creates and starts a distributed application configured for integration testing with Mockoon.
    /// Includes Session container lifetime, isolated volumes, and GoCardless configuration.
    /// </summary>
    /// <param name="testOutput">Optional test output helper for capturing logs.</param>
    /// <param name="mockoonBaseUrl">Base URL of the Mockoon container.</param>
    /// <returns>Started DistributedApplication instance.</returns>
    public static async Task<DistributedApplication> CreateAsync(
        ITestOutputHelper? testOutput,
        string mockoonBaseUrl)
    {
        var builder = await DistributedApplicationTestFactory.CreateAsync(testOutput);

        // Apply standard integration test setup
        builder.WithContainersLifetime(ContainerLifetime.Session);
        builder.WithRandomVolumeNames();

        // Configure Mockoon for Functions resource
        var functionsResource = builder.Resources
            .OfType<ProjectResource>()
            .First(r => r.Name == ResourceNames.Functions);

        functionsResource.Annotations.Add(new EnvironmentCallbackAnnotation(env =>
        {
            env.EnvironmentVariables["Gocardless:BankAccountDataBaseUrl"] = $"{mockoonBaseUrl}/api/v2";
            env.EnvironmentVariables["Gocardless:SecretId"] = "test-secret-id";
            env.EnvironmentVariables["Gocardless:SecretKey"] = "test-secret-key";
        }));

        // Build and start
        var app = await builder.BuildAsync();
        await app.StartAsync();
        await app.WaitForResourcesAsync();

        return app;
    }
}
