using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Networth.Infrastructure.Data.Context;
using Networth.ServiceDefaults;
using Polly;
using Polly.Retry;
using Xunit.Abstractions;

namespace Networth.SystemTests.Infrastructure;

/// <summary>
///     Base class for system tests that require a full Aspire environment.
///     Handles lifecycle management and database cleanup.
/// </summary>
public abstract class SystemTestBase : IAsyncLifetime
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SystemTestBase"/> class.
    /// </summary>
    /// <param name="testOutput">The test output helper.</param>
    protected SystemTestBase(ITestOutputHelper testOutput)
    {
        TestOutput = testOutput;
    }

    /// <summary>
    ///     Gets the test output helper.
    /// </summary>
    protected ITestOutputHelper TestOutput { get; }

    /// <summary>
    ///     Gets the distributed application instance.
    /// </summary>
    protected DistributedApplication App { get; private set; } = null!;

    /// <summary>
    ///     Gets the Networth API client.
    /// </summary>
    protected NetworthClient Client { get; private set; } = null!;

    /// <summary>
    ///     Gets the database connection string.
    /// </summary>
    protected string ConnectionString { get; private set; } = null!;

    /// <inheritdoc />
    public virtual async Task InitializeAsync()
    {
        App = await SystemTestFactory.CreateAsync(TestOutput);
        ConnectionString = await SystemTestFactory.GetDatabaseConnectionStringAsync(App);

        HttpClient functionsClient = App.CreateHttpClient(ResourceNames.Functions);
        Client = new NetworthClient(functionsClient);
    }

    /// <inheritdoc />
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
    /// <returns>A new instance of <see cref="NetworthDbContext"/>.</returns>
    protected NetworthDbContext CreateDbContext()
    {
        return SystemTestFactory.CreateDbContext(ConnectionString);
    }

    /// <summary>
    ///     Waits for a specific log message to appear in the application logs.
    /// </summary>
    /// <param name="logMessage">The log message to wait for.</param>
    /// <param name="timeout">The maximum time to wait. Defaults to 30 seconds.</param>
    protected async Task WaitForLog(string logMessage, TimeSpan? timeout = null)
    {
        var collector = App.Services.GetFakeLogCollector();
        await WaitFor(
            () =>
            {
                var logs = collector.GetSnapshot();
                var found = logs.Any(l => l.Message != null && l.Message.Contains(logMessage, StringComparison.OrdinalIgnoreCase));
                return Task.FromResult(found);
            },
            timeout,
            $"Log message '{logMessage}' not found.");
    }

    /// <summary>
    ///     Waits for a condition to be met within a specified timeout using Polly.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="timeout">The maximum time to wait. Defaults to 30 seconds.</param>
    /// <param name="failureMessage">The message to include in the exception if the timeout is reached.</param>
    protected async Task WaitFor(Func<Task<bool>> condition, TimeSpan? timeout = null, string? failureMessage = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().HandleResult(false),
                MaxRetryAttempts = int.MaxValue, // We control via timeout
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Constant
            })
            .AddTimeout(timeout.Value)
            .Build();

        try
        {
            await pipeline.ExecuteAsync(async token => await condition(), CancellationToken.None);
        }
        catch (TimeoutException)
        {
            throw new Exception(failureMessage ?? $"Condition not met within {timeout.Value.TotalSeconds} seconds.");
        }
    }
}
