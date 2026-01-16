using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.ServiceDefaults;
using Xunit;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Authentication;

/// <summary>
///     Integration tests for mock user authentication configuration.
///     Verifies that when UseAuthentication=false, the mock user is injected correctly.
/// </summary>
public class MockUserAuthenticationTests : IAsyncLifetime, IClassFixture<MockoonTestFixture>
{
    private readonly ITestOutputHelper _testOutput;
    private readonly MockoonTestFixture _mockoonFixture;
    private DistributedApplication _app = null!;
    private HttpClient _httpClient = null!;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MockUserAuthenticationTests"/> class.
    /// </summary>
    public MockUserAuthenticationTests(MockoonTestFixture mockoonFixture, ITestOutputHelper testOutput)
    {
        _mockoonFixture = mockoonFixture;
        _testOutput = testOutput;
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _app = await DistributedApplicationTestFactory.CreateAsync(_testOutput, _mockoonFixture.MockoonBaseUrl);
        _httpClient = _app.CreateHttpClient(ResourceNames.Functions);
    }

    /// <inheritdoc />
    public async Task DisposeAsync() => await _app.DisposeAsync();

    [Fact]
    public async Task GetCurrentUser_WithMockAuthentication_ReturnsUnauthorizedWhenUserNotCreated()
    {
        // Arrange & Act
        // The mock user is injected but the user doesn't exist in the database yet
        var response = await _httpClient.GetAsync("/api/users/me");

        // Assert
        // User is authenticated (mock user injected) but not found in database
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
        Assert.Equal("mock-user-123", jsonDoc.RootElement.GetProperty("firebaseUid").GetString());
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
        Assert.Equal("mock-user-123", jsonDoc.RootElement.GetProperty("firebaseUid").GetString());
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

        // Same Firebase UID across requests
        Assert.Equal(
            jsonDoc1.RootElement.GetProperty("firebaseUid").GetString(),
            jsonDoc2.RootElement.GetProperty("firebaseUid").GetString());

        // Same internal user ID across requests
        Assert.Equal(
            jsonDoc1.RootElement.GetProperty("userId").GetInt32(),
            jsonDoc2.RootElement.GetProperty("userId").GetInt32());
    }
}
