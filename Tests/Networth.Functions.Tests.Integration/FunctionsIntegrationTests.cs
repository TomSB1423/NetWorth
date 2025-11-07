using System.Text.Json;
using Aspire.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Networth.Backend.Functions.Tests.Integration.Fixtures;
using Networth.Backend.Functions.Tests.Integration.Infrastructure;
using Xunit.Abstractions;

namespace Networth.Backend.Functions.Tests.Integration;

/// <summary>
///     Integration tests for the Azure Functions backend using shared Mockoon fixture.
/// </summary>
public class FunctionsIntegrationTests : IClassFixture<MockoonTestFixture>
{
    private readonly MockoonTestFixture _mockoonFixture;
    private readonly ITestOutputHelper _output;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FunctionsIntegrationTests"/> class.
    /// </summary>
    /// <param name="mockoonFixture">The shared Mockoon test fixture.</param>
    /// <param name="output">The test output helper for logging.</param>
    public FunctionsIntegrationTests(MockoonTestFixture mockoonFixture, ITestOutputHelper output)
    {
        _mockoonFixture = mockoonFixture;
        _output = output;
    }

    /// <summary>
    ///     Tests that the GetInstitutions endpoint returns OK status code and valid JSON response.
    /// </summary>
    [Fact]
    public async Task GetInstitutionsEndpointReturnsOkStatusCodeAndValidJson()
    {
        // Arrange
        await using var app = await CreateTestBuilderAsync(_mockoonFixture.MockoonBaseUrl);
        await app.StartAsync();

        // Act
        using var cts = new CancellationTokenSource(TestTimeouts.Default);

        _output.WriteLine("=== Starting Aspire Integration Test ===");
        _output.WriteLine($"Test timeout: {TestTimeouts.Default.TotalMinutes} minutes");
        _output.WriteLine($"Mockoon base URL: {_mockoonFixture.MockoonBaseUrl}");

        // Log initial resource states
        await LogResourceStatesAsync(app, _output);

        // Create HTTP client - the resilience handler will handle retries and timeouts
        var httpClient = app.CreateHttpClient(AspireResourceNames.Functions);
        _output.WriteLine($"HTTP client created for '{AspireResourceNames.Functions}'");

        // Make request - the resilience handler will retry until success or timeout
        _output.WriteLine("Sending GET request to /api/institutions...");
        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync("/api/institutions", cts.Token);
            _output.WriteLine($"Got response: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Request failed with exception: {ex.GetType().Name}: {ex.Message}");
            await LogResourceStatesAsync(app, _output);
            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            _output.WriteLine($"Request returned non-success status: {response.StatusCode}");
            await LogResourceStatesAsync(app, _output);
        }

        _output.WriteLine("=== Test Successful - Endpoint Ready ===");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(cts.Token);
        Assert.False(string.IsNullOrWhiteSpace(content), "Response content should not be empty");

        using var jsonDoc = JsonDocument.Parse(content);
        var institutionsElement = jsonDoc.RootElement.ValueKind == JsonValueKind.Array
            ? jsonDoc.RootElement
            : jsonDoc.RootElement.GetProperty("value");

        var institutions = institutionsElement.EnumerateArray().ToList();
        Assert.True(institutions.Count > 0, "Should return at least one institution");

        var firstInstitution = institutions.First();
        Assert.True(firstInstitution.TryGetProperty("id", out JsonElement _), "Institution should have 'id' property");
        Assert.True(firstInstitution.TryGetProperty("name", out JsonElement _), "Institution should have 'name' property");

        // Verify we got mock data from Mockoon
        var firstInstitutionId = firstInstitution.GetProperty("id").GetString();
        Assert.Contains("ABNAMRO_ABNAGB2LXXX", firstInstitutionId ?? string.Empty);

        _output.WriteLine("=== All Assertions Passed ===");
    }

    /// <summary>
    ///     Creates a configured Aspire application builder for testing.
    /// </summary>
    /// <param name="mockoonBaseUrl">The base URL for the Mockoon API.</param>
    /// <returns>A configured distributed application.</returns>
    private static async Task<DistributedApplication> CreateTestBuilderAsync(string mockoonBaseUrl)
    {
        // Use a longer timeout for integration tests to allow for slow startup and migrations
        TimeSpan defaultTimeout = TimeSpan.FromSeconds(600); // 10 minutes

        var appBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Networth_AppHost>(CancellationToken.None);

        // Configure Mockoon URL instead of real GoCardless
        appBuilder.Configuration[GoCardlessConfiguration.BankAccountDataBaseUrl] = mockoonBaseUrl;
        appBuilder.Configuration[GoCardlessConfiguration.SecretId] = GoCardlessConfiguration.TestSecretId;
        appBuilder.Configuration[GoCardlessConfiguration.SecretKey] = GoCardlessConfiguration.TestSecretKey;

        // Configure HTTP client resilience with extended timeouts for integration tests
        appBuilder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler(b =>
            {
                b.AttemptTimeout = b.TotalRequestTimeout = new HttpTimeoutStrategyOptions()
                {
                    Timeout = defaultTimeout,
                };
                b.CircuitBreaker = new HttpCircuitBreakerStrategyOptions()
                {
                    SamplingDuration = defaultTimeout * 2,
                };
            });
        });

        // Configure logging
        appBuilder.Services.AddLogging(logging => logging
            .AddConsole()
            .AddFilter("Default", LogLevel.Information)
            .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
            .AddFilter("Aspire.Hosting.Dcp", LogLevel.Warning));

        return await appBuilder.BuildAsync();
    }

    /// <summary>
    ///     Logs the current state of all resources in the Aspire app.
    /// </summary>
    private static async Task LogResourceStatesAsync(DistributedApplication app, ITestOutputHelper output)
    {
        output.WriteLine("--- Resource States ---");

        try
        {
            var resourceNames = new[] { "postgres", "networth-db", "functions", "funcstoragecf0b0" };

            foreach (var resourceName in resourceNames)
            {
                try
                {
                    // Try to wait for the resource with a short timeout to see if it responds
                    await app.ResourceNotifications.WaitForResourceAsync(
                            resourceName,
                            _ => true,
                            CancellationToken.None)
                        .WaitAsync(TimeSpan.FromMilliseconds(500));

                    output.WriteLine($"  {resourceName}: Found and responsive");
                }
                catch (TimeoutException)
                {
                    output.WriteLine($"  {resourceName}: Timeout - may not be ready");
                }
                catch (Exception ex)
                {
                    output.WriteLine($"  {resourceName}: ERROR - {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            output.WriteLine($"ERROR logging resource states: {ex.Message}");
        }

        output.WriteLine("--- End Resource States ---");
    }
}
