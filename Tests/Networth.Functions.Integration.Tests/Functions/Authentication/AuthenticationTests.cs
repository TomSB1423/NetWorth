using System.Net;
using Networth.Functions.Models.Responses;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Functions.Authentication;

public class AuthenticationTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    [Fact]
    public async Task GetCurrentUser_WithFakeAuthentication_ReturnsUser()
    {
        // Arrange - Use the FakeJwtBearer format for authentication
        var userId = "1234567890";
        var name = "John Doe";
        AuthenticateAs(userId, name);

        // Act
        var user = await Client.GetCurrentUserAsync();

        // Assert
        Assert.NotNull(user);
        Assert.True(user.IsAuthenticated);
        Assert.Equal(userId, user.UserId);
        Assert.Equal(name, user.Name);
    }

    protected override Task<DistributedApplication> CreateAppAsync() =>
        DistributedApplicationTestFactory.CreateAsync(
            TestOutput,
            MockoonFixture.MockoonBaseUrl,
            useMockAuthentication: true);
}
