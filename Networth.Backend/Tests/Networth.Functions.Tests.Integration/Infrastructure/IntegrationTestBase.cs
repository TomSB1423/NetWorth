using Aspire.Hosting;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace Networth.Backend.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Base class for integration tests that provides Mockoon container setup and Aspire configuration.
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private IContainer? mockoonContainer;

    /// <summary>
    ///     Gets the Mockoon base URL for API calls.
    /// </summary>
    protected string MockoonBaseUrl { get; private set; } = string.Empty;

    /// <summary>
    ///     Initializes the test by starting the Mockoon container.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InitializeAsync()
    {
        mockoonContainer = new ContainerBuilder()
            .WithImage(MockoonConfiguration.Image)
            .WithPortBinding(MockoonConfiguration.Port, true)
            .WithBindMount(
                Path.Combine(Directory.GetCurrentDirectory(), MockoonConfiguration.GoCardlessDataFile),
                MockoonConfiguration.ContainerDataPath)
            .WithCommand("--data", MockoonConfiguration.ContainerDataPath, "--port", MockoonConfiguration.Port.ToString())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MockoonConfiguration.Port))
            .Build();

        await mockoonContainer.StartAsync();

        MockoonBaseUrl = $"http://{mockoonContainer.Hostname}:{mockoonContainer.GetMappedPublicPort(MockoonConfiguration.Port)}";

        // Poll Mockoon until it's ready
        await WaitForMockoonReadyAsync();
    }

    /// <summary>
    ///     Cleans up resources by stopping the Mockoon container.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DisposeAsync()
    {
        if (mockoonContainer != null)
        {
            await mockoonContainer.StopAsync();
            await mockoonContainer.DisposeAsync();
        }
    }

    /// <summary>
    ///     Creates a configured Aspire application builder for testing.
    /// </summary>
    /// <returns>A configured distributed application builder.</returns>
    protected async Task<DistributedApplication> CreateTestBuilderAsync()
    {
        var appBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync(typeof(Projects.Networth_AppHost));

        // Configure Mockoon URL instead of real GoCardless
        appBuilder.Configuration[GoCardlessConfiguration.BankAccountDataBaseUrl] = MockoonBaseUrl;
        appBuilder.Configuration[GoCardlessConfiguration.SecretId] = GoCardlessConfiguration.TestSecretId;
        appBuilder.Configuration[GoCardlessConfiguration.SecretKey] = GoCardlessConfiguration.TestSecretKey;

        // Configure HTTP client resilience
        appBuilder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        // Configure logging
        appBuilder.Services.AddLogging(logging => logging
            .AddConsole()
            .AddFilter("Default", LogLevel.Information)
            .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
            .AddFilter("Aspire.Hosting.Dcp", LogLevel.Warning));

        return await appBuilder.BuildAsync();
    }

    /// <summary>
    ///     Polls Mockoon until it responds successfully.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task WaitForMockoonReadyAsync()
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        var maxAttempts = 60; // 60 attempts * 6 seconds = 360 seconds max wait
        var attempt = 0;

        while (attempt < maxAttempts)
        {
            try
            {
                var response = await httpClient.GetAsync($"{MockoonBaseUrl}/api/v2/institutions/");
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Expected during startup
            }
            catch (TaskCanceledException)
            {
                // Timeout - try again
            }

            attempt++;
            await Task.Delay(6000);
        }

        throw new TimeoutException($"Mockoon failed to become ready after {maxAttempts} attempts");
    }
}
