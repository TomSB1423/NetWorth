using System.Text.Json;
using Aspire.Hosting;
using MyApp.AppHost;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Projects;

namespace Networth.Functions.Tests.Integration;

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

        using var cts = new CancellationTokenSource(TestTimeouts.Default);

        // Wait for all resources to be healthy
        await app.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Postgres, cts.Token)
            .WaitAsync(TestTimeouts.Default, cts.Token);
        await app.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.NetworthDb, cts.Token)
            .WaitAsync(TestTimeouts.Default, cts.Token);
        await app.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Functions, cts.Token)
            .WaitAsync(TestTimeouts.Default, cts.Token);

        // Act
        HttpClient httpClient = app.CreateHttpClient(ResourceNames.Functions);
        HttpResponseMessage response = await httpClient.GetAsync("/api/institutions", cts.Token);

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
            .CreateAsync<Networth_AppHost>(CancellationToken.None);

        // Configure Mockoon URL instead of real GoCardless
        appBuilder.Configuration[GoCardlessConfiguration.BankAccountDataBaseUrl] = mockoonBaseUrl;
        appBuilder.Configuration[GoCardlessConfiguration.SecretId] = GoCardlessConfiguration.TestSecretId;
        appBuilder.Configuration[GoCardlessConfiguration.SecretKey] = GoCardlessConfiguration.TestSecretKey;


        return await appBuilder.BuildAsync();
    }
}
