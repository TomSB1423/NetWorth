using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration;

/// <summary>
///     Tests for the AppHost configuration and infrastructure.
/// </summary>
[Collection("Integration")]
public class AppHostTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    [Fact]
    public void AppHostRunsCleanly()
    {
        // The AppHost is started in InitializeAsync.
        // If it fails to start, this test will fail during setup.
        Assert.NotNull(App);
    }
}
