using System.Net;
using System.Text.Json;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Authentication;

/// <summary>
///     Integration tests for mock user authentication configuration.
///     Verifies that when UseAuthentication=false, the mock user is injected correctly.
/// </summary>
[Collection("Integration")]
public class MockUserAuthenticationTests : IntegrationTestBase
{
    private HttpClient _httpClient = null!;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MockUserAuthenticationTests"/> class.
    /// </summary>
    public MockUserAuthenticationTests(MockoonTestFixture mockoonFixture, ITestOutputHelper testOutput)
        : base(mockoonFixture, testOutput)
    {
    }

    /// <inheritdoc />
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _httpClient = App.CreateHttpClient(ResourceNames.Functions);

        // Ensure database schema is created
        await EnsureDatabaseMigratedAsync();
    }

    [Fact]
    public async Task GetCurrentUser_WithMockAuthentication_ReturnsSeededMockUser()
    {
        // Arrange & Act
        // The mock user is seeded in the database by the InitialCreate migration
        var response = await _httpClient.GetAsync("/api/users/me");

        // Assert
        // User is authenticated (mock user injected) and found in database (seeded)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithMockAuthentication_UsesConfiguredMockUserDetails()
    {
        // Arrange & Act
        var response = await _httpClient.PostAsync("/api/users", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);

        // Verify the mock user details from settings.json are used
        Assert.Equal("Mock Development User", jsonDoc.RootElement.GetProperty("name").GetString());
        Assert.Equal("mock@example.com", jsonDoc.RootElement.GetProperty("email").GetString());
    }

    [Fact]
    public async Task GetCurrentUser_AfterCreateUser_ReturnsMockUserDetails()
    {
        // Arrange - Create the user first
        var createResponse = await _httpClient.PostAsync("/api/users", null);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        // Act - Get the current user
        var response = await _httpClient.GetAsync("/api/users/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);

        // Verify the mock user details match the configured values
        Assert.Equal("Mock Development User", jsonDoc.RootElement.GetProperty("name").GetString());
        Assert.Equal("mock@example.com", jsonDoc.RootElement.GetProperty("email").GetString());
    }

    [Fact]
    public async Task ProtectedEndpoint_WithMockAuthentication_DoesNotRequireAuthorizationHeader()
    {
        // Arrange - Create user first so we have an authenticated user in the database
        await _httpClient.PostAsync("/api/users", null);

        // Act - Call a protected endpoint without any Authorization header
        var response = await _httpClient.GetAsync("/api/users/me");

        // Assert - Should succeed because mock authentication injects user automatically
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MockUser_ClaimsAreConsistent_AcrossMultipleRequests()
    {
        // Arrange - Create the user
        await _httpClient.PostAsync("/api/users", null);

        // Act - Make multiple requests
        var response1 = await _httpClient.GetAsync("/api/users/me");
        var response2 = await _httpClient.GetAsync("/api/users/me");

        // Assert - Both should return the same user
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();

        using var jsonDoc1 = JsonDocument.Parse(content1);
        using var jsonDoc2 = JsonDocument.Parse(content2);

        // Same internal user ID across requests
        Assert.Equal(
            jsonDoc1.RootElement.GetProperty("userId").GetGuid(),
            jsonDoc2.RootElement.GetProperty("userId").GetGuid());

        // Same email across requests
        Assert.Equal(
            jsonDoc1.RootElement.GetProperty("email").GetString(),
            jsonDoc2.RootElement.GetProperty("email").GetString());
    }
}
