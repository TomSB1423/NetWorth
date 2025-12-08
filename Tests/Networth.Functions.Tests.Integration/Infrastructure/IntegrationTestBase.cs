using Networth.Functions.Tests.Integration.Fixtures;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Base class for integration tests that require a distributed application.
///     Handles the lifecycle of the application and provides Mockoon support.
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime, IClassFixture<MockoonTestFixture>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IntegrationTestBase"/> class.
    /// </summary>
    /// <param name="mockoonFixture">The Mockoon fixture.</param>
    /// <param name="testOutput">The test output helper.</param>
    protected IntegrationTestBase(MockoonTestFixture mockoonFixture, ITestOutputHelper testOutput)
    {
        MockoonFixture = mockoonFixture;
        TestOutput = testOutput;
    }

    /// <summary>
    ///     Gets the test output helper.
    /// </summary>
    protected ITestOutputHelper TestOutput { get; }

    /// <summary>
    ///     Gets the Mockoon fixture.
    /// </summary>
    protected MockoonTestFixture MockoonFixture { get; }

    /// <summary>
    ///     Gets the distributed application instance.
    /// </summary>
    protected DistributedApplication App { get; private set; } = null!;

    /// <inheritdoc />
    public virtual async Task InitializeAsync()
    {
        App = await CreateAppAsync();
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
    ///     Creates the distributed application instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the distributed application.</returns>
    protected virtual Task<DistributedApplication> CreateAppAsync()
    {
        return DistributedApplicationTestFactory.CreateAsync(TestOutput, MockoonFixture.MockoonBaseUrl);
    }
}
