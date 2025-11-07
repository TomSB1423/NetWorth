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

        // Wait for database to be ready first
        await app.ResourceNotifications.WaitForResourceHealthyAsync("networth-db", cts.Token);

        // Create HTTP client and wait for Functions to be ready by polling the endpoint
        var httpClient = app.CreateHttpClient(AspireResourceNames.Functions);

        // Poll the endpoint until it responds (Functions app might still be migrating database)
        var maxAttempts = 30; // 30 attempts with 2 second delays = 60 seconds max
        var delayBetweenAttempts = TimeSpan.FromSeconds(2);
        HttpResponseMessage? response = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                response = await httpClient.GetAsync("/api/institutions", cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    break;
                }
            }
            catch (HttpRequestException)
            {
                // Functions app not ready yet
                if (attempt == maxAttempts)
                {
                    throw;
                }
            }
            catch (TaskCanceledException)
            {
                // Timeout on individual request
                if (attempt == maxAttempts)
                {
                    throw;
                }
            }

            await Task.Delay(delayBetweenAttempts, cts.Token);
        }

        // Assert
        Assert.NotNull(response);
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
