using System.Text.Json;
using Networth.Backend.Functions.Tests.Integration.Infrastructure;

namespace Networth.Backend.Functions.Tests.Integration;

/// <summary>
///     Integration tests for the Azure Functions backend.
/// </summary>
public class FunctionsIntegrationTests : IntegrationTestBase
{
    /// <summary>
    ///     Tests that the GetInstitutions endpoint returns OK status code and valid JSON response.
    /// </summary>
    [Fact]
    public async Task GetInstitutionsEndpointReturnsOkStatusCodeAndValidJson()
    {
        // Arrange
        await using var app = await CreateTestBuilderAsync();
        await app.StartAsync();

        // Act
        using var cts = new CancellationTokenSource(TestTimeouts.Default);

        await app.ResourceNotifications.WaitForResourceHealthyAsync(AspireResourceNames.Functions, cts.Token);

        var httpClient = app.CreateHttpClient(AspireResourceNames.Functions);
        var response = await httpClient.GetAsync("/api/institutions", cts.Token);

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
}
