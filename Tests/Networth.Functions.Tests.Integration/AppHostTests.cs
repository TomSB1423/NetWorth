using Networth.Functions.Tests.Integration.Infrastructure;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration;

public class AppHostTests(ITestOutputHelper testOutput)
{
    private static readonly TimeSpan BuildStopTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan StartStopTimeout = TimeSpan.FromSeconds(120);

    [Fact]
    public async Task AppHostRunsCleanly()
    {
        IDistributedApplicationTestingBuilder appHost = await DistributedApplicationTestFactory.CreateAsync(testOutput);
        await using var app = await appHost.BuildAsync().WaitAsync(BuildStopTimeout);

        await app.StartAsync().WaitAsync(StartStopTimeout);
        await app.WaitForResourcesAsync().WaitAsync(StartStopTimeout);

        app.EnsureNoErrorsLogged();

        await app.StopAsync().WaitAsync(BuildStopTimeout);
    }
}
