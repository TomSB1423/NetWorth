using System.Text.Json;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration;

/// <summary>
///     Integration tests for the Azure Functions backend using shared Mockoon fixture.
/// </summary>
public class FunctionsIntegrationTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput) : IClassFixture<MockoonTestFixture>
{
    private static readonly TimeSpan BuildStopTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan StartStopTimeout = TimeSpan.FromSeconds(120);

    /// <summary>
    ///     Tests that the GetInstitutions endpoint returns OK status code and valid JSON response.
    /// </summary>
    [Fact]
    public async Task GetInstitutionsEndpointReturnsOkStatusCodeAndValidJson()
    {
        // Arrange
        await using IDistributedApplicationTestingBuilder appHost = await DistributedApplicationTestFactory.CreateAsync(testOutput);
        appHost.ConfigureMockoonForResource(mockoonTestFixture.MockoonBaseUrl);
        await using var app = await appHost.BuildAsync().WaitAsync(BuildStopTimeout);

        await app.StartAsync().WaitAsync(StartStopTimeout);
        await app.WaitForResourcesAsync().WaitAsync(StartStopTimeout);

        // Act
        HttpClient httpClient = app.CreateHttpClient(ResourceNames.Functions, true);
        var response = await httpClient.GetAsync("/api/institutions", CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync(CancellationToken.None);
        Assert.False(string.IsNullOrWhiteSpace(content), "Response content should not be empty");

        using var jsonDoc = JsonDocument.Parse(content);
        var institutionsElement = jsonDoc.RootElement.ValueKind == JsonValueKind.Array
            ? jsonDoc.RootElement
            : jsonDoc.RootElement.GetProperty("value");

        var institutions = institutionsElement.EnumerateArray().ToList();
        Assert.True(institutions.Count > 0, "Should return at least one institution");

        var firstInstitution = institutions.First();
        Assert.True(firstInstitution.TryGetProperty("id", out _), "Institution should have 'id' property");
        Assert.True(firstInstitution.TryGetProperty("name", out _), "Institution should have 'name' property");

        var firstInstitutionId = firstInstitution.GetProperty("id").GetString();
        Assert.Contains("SANDBOXFINANCE_SFIN0000", firstInstitutionId ?? string.Empty);
    }
}
