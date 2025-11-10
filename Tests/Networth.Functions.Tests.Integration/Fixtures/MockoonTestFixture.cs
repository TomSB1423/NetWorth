using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Networth.Functions.Tests.Integration.Infrastructure;

namespace Networth.Functions.Tests.Integration.Fixtures;

/// <summary>
///     Test fixture providing Mockoon container for GoCardless API mocking.
///     Shared across all tests in a test class via IClassFixture.
/// </summary>
public class MockoonTestFixture : IAsyncLifetime
{
    /// <summary>
    ///     Gets the Mockoon base URL for API calls.
    /// </summary>
    public string MockoonBaseUrl { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the Mockoon container instance.
    /// </summary>
    public IContainer? Container { get; private set; }

    /// <summary>
    ///     Initializes the fixture by starting the Mockoon container.
    /// </summary>
    public async Task InitializeAsync()
    {
        Container = new ContainerBuilder()
            .WithImage(MockoonConfiguration.Image)
            .WithPortBinding(MockoonConfiguration.Port, true)
            .WithBindMount(
                Path.Combine(Directory.GetCurrentDirectory(), MockoonConfiguration.GoCardlessDataFile),
                MockoonConfiguration.ContainerDataPath)
            .WithCommand("--data", MockoonConfiguration.ContainerDataPath, "--port", MockoonConfiguration.Port.ToString())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilExternalTcpPortIsAvailable(MockoonConfiguration.Port))
            .Build();

        await Container.StartAsync();

        MockoonBaseUrl = $"http://{Container.Hostname}:{Container.GetMappedPublicPort(MockoonConfiguration.Port)}";
    }

    /// <summary>
    ///     Disposes the fixture by stopping the Mockoon container.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (Container != null)
        {
            await Container.StopAsync();
            await Container.DisposeAsync();
        }
    }
}
