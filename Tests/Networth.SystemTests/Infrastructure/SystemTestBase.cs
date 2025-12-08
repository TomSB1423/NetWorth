using Networth.Infrastructure.Data.Context;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.SystemTests.Infrastructure;

/// <summary>
///     Base class for system tests that require a full Aspire environment.
///     Handles lifecycle management and database cleanup.
/// </summary>
public abstract class SystemTestBase : IAsyncLifetime
{
    protected SystemTestBase(ITestOutputHelper testOutput)
    {
        TestOutput = testOutput;
    }

    protected ITestOutputHelper TestOutput { get; }

    protected DistributedApplication App { get; private set; } = null!;

    protected NetworthClient Client { get; private set; } = null!;

    protected string ConnectionString { get; private set; } = null!;

    public virtual async Task InitializeAsync()
    {
        App = await SystemTestFactory.CreateAsync(TestOutput);
        ConnectionString = await SystemTestFactory.GetDatabaseConnectionStringAsync(App);

        // Ensure clean database before each test
        await SystemTestFactory.ResetDatabaseAsync(ConnectionString);

        HttpClient functionsClient = App.CreateHttpClient(ResourceNames.Functions);
        Client = new NetworthClient(functionsClient);
    }

    public virtual async Task DisposeAsync()
    {
        if (App != null)
        {
            await App.DisposeAsync();
        }
    }

    /// <summary>
    ///     Creates a DbContext for direct database access in tests.
    /// </summary>
    protected NetworthDbContext CreateDbContext()
    {
        return SystemTestFactory.CreateDbContext(ConnectionString);
    }
}
