using System.Text;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
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
    protected MockoonTestFixture MockoonFixture { get; }

    /// <inheritdoc />
    public virtual async Task InitializeAsync()
    {
        App = await CreateAppAsync();

        // Use direct HttpClient without Aspire's proxy/factory to avoid any auth middleware
        var endpoint = App.GetEndpoint(ResourceNames.Functions);
        var httpClient = new HttpClient { BaseAddress = endpoint };

        Client = new NetworthClient(httpClient);
    }

    /// <inheritdoc />
    public virtual async Task DisposeAsync() => await App.DisposeAsync();

    /// <summary>
    ///     Sets up fake authentication for the specified user.
    ///     Uses the FakeJwtBearer library format which sends JSON-encoded claims.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="name">The user name.</param>
    protected void AuthenticateAs(string userId, string name = "Test User")
    {
        Client.SetFakeAuthToken(userId, $"{userId}@test.com", name);
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
    protected virtual Task<DistributedApplication> CreateAppAsync() =>
        DistributedApplicationTestFactory.CreateAsync(TestOutput, MockoonFixture.MockoonBaseUrl, useMockAuthentication: true);
}
