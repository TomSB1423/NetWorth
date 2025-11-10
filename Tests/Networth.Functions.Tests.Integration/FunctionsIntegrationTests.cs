using System.Text.Json;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration;

/// <summary>
///     Integration tests for the Azure Functions backend using shared Mockoon fixture.
/// </summary>
public class FunctionsIntegrationTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IClassFixture<MockoonTestFixture>
{
    /// <summary>
    ///     Tests that the GetInstitutions endpoint returns OK status code and valid JSON response.
    /// </summary>
    [Fact]
    public async Task GetInstitutionsEndpointReturnsOkStatusCodeAndValidJson()
    {
        // Arrange
        await using var app = await DistributedApplicationTestFactory.CreateAsync(
            testOutput,
            mockoonTestFixture.MockoonBaseUrl);

        // Act
        var client = app.CreateHttpClient(ResourceNames.Functions);
        var response = await client.GetAsync("/api/institutions", CancellationToken.None);

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

    /// <summary>
    ///     Downloads the OpenAPI spec from the Functions app and saves it to the workspace root.
    ///     This is used by CI to validate the spec with Spectral.
    /// </summary>
    [Fact]
    [Trait("Category", "OpenAPI")]
    public async Task DownloadOpenApiSpecification()
    {
        // Arrange
        await using var app = await DistributedApplicationTestFactory.CreateAsync(
            testOutput,
            mockoonTestFixture.MockoonBaseUrl);

        var client = app.CreateHttpClient(ResourceNames.Functions);

        // Act
        var response = await client.GetAsync("/api/swagger.json", CancellationToken.None);

        // Assert
        Assert.True(response.IsSuccessStatusCode, "Failed to download OpenAPI spec");

        var specContent = await response.Content.ReadAsStringAsync(CancellationToken.None);
        Assert.NotNull(specContent);

        // Validate it's valid JSON
        using var jsonDoc = JsonDocument.Parse(specContent);
        Assert.True(
            jsonDoc.RootElement.TryGetProperty("openapi", out _) ||
            jsonDoc.RootElement.TryGetProperty("swagger", out _),
            "Spec should contain 'openapi' or 'swagger' property");

        // Save to workspace root for CI validation
        var workspaceRoot = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE") ?? Directory.GetCurrentDirectory();
        var outputPath = Path.Combine(workspaceRoot, "openapi.json");
        await File.WriteAllTextAsync(outputPath, specContent, CancellationToken.None);
        testOutput.WriteLine($"OpenAPI spec downloaded from /api/swagger.json and saved to {outputPath}");
    }
}
