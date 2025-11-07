using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Networth.Functions.Tests.Integration.Infrastructure;

namespace Networth.Functions.Tests.Integration.Fixtures;

/// <summary>
/// Test fixture providing Mockoon container for GoCardless API mocking.
/// Shared across all tests in a test class via IClassFixture.
/// </summary>
public class MockoonTestFixture : IAsyncLifetime
{
    private IContainer? _mockoonContainer;

    /// <summary>
    /// Gets the Mockoon base URL for API calls.
    /// </summary>
    public string MockoonBaseUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the Mockoon container instance.
    /// </summary>
    public IContainer? Container => _mockoonContainer;

    /// <summary>
    /// Initializes the fixture by starting the Mockoon container.
    /// </summary>
    public async Task InitializeAsync()
    {
        _mockoonContainer = new ContainerBuilder()
            .WithImage(MockoonConfiguration.Image)
            .WithPortBinding(MockoonConfiguration.Port, true)
            .WithBindMount(
                Path.Combine(Directory.GetCurrentDirectory(), MockoonConfiguration.GoCardlessDataFile),
                MockoonConfiguration.ContainerDataPath)
            .WithCommand("--data", MockoonConfiguration.ContainerDataPath, "--port", MockoonConfiguration.Port.ToString())
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(MockoonConfiguration.Port)
                    .ForPath("/ready")
                    .ForStatusCode(System.Net.HttpStatusCode.OK)))
            .Build();

        await _mockoonContainer.StartAsync();

        MockoonBaseUrl = $"http://{_mockoonContainer.Hostname}:{_mockoonContainer.GetMappedPublicPort(MockoonConfiguration.Port)}";
    }

    /// <summary>
    /// Disposes the fixture by stopping the Mockoon container.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_mockoonContainer != null)
        {
            await _mockoonContainer.StopAsync();
            await _mockoonContainer.DisposeAsync();
        }
    }
}
