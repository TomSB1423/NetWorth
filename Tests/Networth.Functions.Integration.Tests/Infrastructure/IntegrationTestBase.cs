using System.Text;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Infrastructure.Data.Context;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Base class for integration tests that require a distributed application.
///     Handles the lifecycle of the application and provides Mockoon support.
///     The MockoonTestFixture is shared via ICollectionFixture in IntegrationTestCollection.
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
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
    ///     Gets the distributed application instance.
    /// </summary>
    protected DistributedApplication App { get; private set; } = null!;

    /// <summary>
    ///     Gets the Networth API client.
    /// </summary>
    protected NetworthClient Client { get; private set; } = null!;

    /// <summary>
    ///     Gets the test output helper.
    /// </summary>
    protected ITestOutputHelper TestOutput { get; }

    /// <summary>
    ///     Gets the Mockoon fixture.
    /// </summary>
    private MockoonTestFixture MockoonFixture { get; }

    /// <inheritdoc />
    public virtual async Task InitializeAsync()
    {
        App = await CreateAppAsync();
        var httpClient = App.CreateHttpClient(ResourceNames.Functions);
        Client = new NetworthClient(httpClient);
    }

    /// <inheritdoc />
    public virtual async Task DisposeAsync() => await App.DisposeAsync();

    /// <summary>
    ///     Ensures the database is migrated and ready for tests.
    ///     Call this in test InitializeAsync when database operations are needed.
    /// </summary>
    protected async Task EnsureDatabaseMigratedAsync()
    {
        string? dbConnectionString = await App.GetConnectionStringAsync(ResourceNames.NetworthDb);
        TestOutput.WriteLine($"Database connection string: {dbConnectionString}");

        ServiceCollection services = new();
        services.AddDbContext<NetworthDbContext>(options =>
        {
            options.UseNpgsql(dbConnectionString);
            options.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        await using ServiceProvider serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        TestOutput.WriteLine("Starting database migration...");
        await dbContext.Database.MigrateAsync();
        TestOutput.WriteLine("Database migration completed.");
    }

    /// <summary>
    ///     Polls the specified queue for a message containing the expected text.
    /// </summary>
    /// <param name="queueName">The name of the queue to poll.</param>
    /// <param name="expectedText">The text expected to be contained in the message.</param>
    /// <param name="timeoutSeconds">The maximum time to wait for the message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task AssertQueueMessageReceivedAsync(string queueName, string expectedText, int timeoutSeconds = 10)
    {
        string? queueConnectionString = await App.GetConnectionStringAsync(ResourceNames.Queues);
        QueueClient queueClient = new(queueConnectionString, queueName);
        await queueClient.CreateIfNotExistsAsync();

        DateTime startTime = DateTime.UtcNow;
        while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(timeoutSeconds))
        {
            Response<QueueMessage[]>? response = await queueClient.ReceiveMessagesAsync();
            QueueMessage[] msgs = response.Value;

            if (msgs.Length > 0)
            {
                string? messageText = msgs[0].MessageText;
                try
                {
                    byte[] bytes = Convert.FromBase64String(messageText);
                    messageText = Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    // ignore if not base64
                }

                if (messageText.Contains(expectedText))
                {
                    return;
                }
            }

            await Task.Delay(200);
        }

        Assert.Fail($"Message containing '{expectedText}' was not found in queue '{queueName}' within {timeoutSeconds} seconds.");
    }

    /// <summary>
    ///     Creates the distributed application instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the distributed application.</returns>
    private Task<DistributedApplication> CreateAppAsync() =>
        DistributedApplicationTestFactory.CreateAsync(TestOutput, MockoonFixture.MockoonBaseUrl);
}
