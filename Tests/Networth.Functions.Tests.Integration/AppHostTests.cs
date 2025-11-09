using Aspire.Hosting.Testing;
using Networth.Functions.Tests.Integration.Infrastructure;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration;

/// <summary>
///     Tests for the AppHost configuration and infrastructure.
/// </summary>
public class AppHostTests(ITestOutputHelper testOutput)
{
    [Fact]
    public async Task AppHostRunsCleanly()
    {
        // Arrange
        var builder = await DistributedApplicationTestFactory.CreateAsync(testOutput);
        builder.WithContainersLifetime(ContainerLifetime.Session);
        builder.WithRandomVolumeNames();

        // Act
        await using var app = await builder.BuildAsync();
        await app.StartAsync();
        await app.WaitForResourcesAsync();

        // Assert
        app.EnsureNoErrorsLogged();
    }
}
