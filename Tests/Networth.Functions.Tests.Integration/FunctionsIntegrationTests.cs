using System.Text.Json;
using Aspire.Hosting;
using Microsoft.Extensions.Logging;
using Networth.Backend.Functions.Tests.Integration.Fixtures;
using Networth.Backend.Functions.Tests.Integration.Infrastructure;

namespace Networth.Backend.Functions.Tests.Integration;

/// <summary>
///     Integration tests for the Azure Functions backend using shared Mockoon fixture.
/// </summary>
public class FunctionsIntegrationTests : IClassFixture<MockoonTestFixture>
{
    private readonly MockoonTestFixture _mockoonFixture;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FunctionsIntegrationTests"/> class.
    /// </summary>
    /// <param name="mockoonFixture">The shared Mockoon test fixture.</param>
    public FunctionsIntegrationTests(MockoonTestFixture mockoonFixture)
    {
        _mockoonFixture = mockoonFixture;
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

        // Create HTTP client and wait for Functions to be ready by polling the endpoint
        // Note: We don't wait for database explicitly as the Functions endpoint won't work until DB is ready anyway
        var httpClient = app.CreateHttpClient(AspireResourceNames.Functions);
        httpClient.Timeout = TimeSpan.FromSeconds(10); // Set per-request timeout

        // Poll the endpoint until it responds (Functions app might still be migrating database)
        // Use 3 minutes of polling to leave buffer before the 5-minute test timeout
        var maxAttempts = 36; // 36 attempts with 5 second delays = 180 seconds (3 minutes) max
        var delayBetweenAttempts = TimeSpan.FromSeconds(5);
        HttpResponseMessage? response = null;
        var lastException = default(Exception);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cts.Token.ThrowIfCancellationRequested();

            try
            {
                using var requestCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                requestCts.CancelAfter(TimeSpan.FromSeconds(10));

                response = await httpClient.GetAsync("/api/institutions", requestCts.Token);
                if (response.IsSuccessStatusCode)
                {
                    // Success!
                    break;
                }

                lastException = new InvalidOperationException($"Attempt {attempt}: Got status code {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                // Functions app not ready yet, continue polling
                lastException = ex;
            }
            catch (TaskCanceledException ex) when (!cts.Token.IsCancellationRequested)
            {
                // Request timeout (not overall timeout), continue polling
                lastException = ex;
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                // Overall test timeout reached
                throw new TimeoutException($"Test timeout reached after {attempt} attempts. Functions endpoint never became ready.", lastException);
            }

            if (attempt < maxAttempts)
            {
                try
                {
                    await Task.Delay(delayBetweenAttempts, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Overall test timeout reached during delay
                    throw new TimeoutException($"Test timeout reached after {attempt} attempts while waiting between retries. Functions endpoint never became ready.", lastException);
                }
            }
        }

        if (response == null || !response.IsSuccessStatusCode)
        {
            var statusInfo = response != null ? $"Status: {response.StatusCode}" : "No response received";
            throw new InvalidOperationException(
                $"Functions endpoint did not become ready after {maxAttempts} attempts. {statusInfo}",
                lastException);
        }

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
    }

    /// <summary>
    ///     Creates a configured Aspire application builder for testing.
    /// </summary>
    /// <param name="mockoonBaseUrl">The base URL for the Mockoon API.</param>
    /// <returns>A configured distributed application.</returns>
    private static async Task<DistributedApplication> CreateTestBuilderAsync(string mockoonBaseUrl)
    {
        var appBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync(typeof(Projects.Networth_AppHost));

        // Configure Mockoon URL instead of real GoCardless
        appBuilder.Configuration[GoCardlessConfiguration.BankAccountDataBaseUrl] = mockoonBaseUrl;
        appBuilder.Configuration[GoCardlessConfiguration.SecretId] = GoCardlessConfiguration.TestSecretId;
        appBuilder.Configuration[GoCardlessConfiguration.SecretKey] = GoCardlessConfiguration.TestSecretKey;

        // Note: We don't add the standard resilience handler here as it has a 30-second timeout
        // which can cause issues during initial startup when migrations are running.
        // The test itself handles retries with appropriate delays.

        // Configure logging
        appBuilder.Services.AddLogging(logging => logging
            .AddConsole()
            .AddFilter("Default", LogLevel.Information)
            .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
            .AddFilter("Aspire.Hosting.Dcp", LogLevel.Warning));

        return await appBuilder.BuildAsync();
    }
}
