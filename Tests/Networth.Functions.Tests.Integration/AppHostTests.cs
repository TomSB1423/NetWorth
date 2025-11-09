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
        // Arrange & Act
        await using var app = await DistributedApplicationTestFactory.CreateAsync(testOutput);
    }
}
