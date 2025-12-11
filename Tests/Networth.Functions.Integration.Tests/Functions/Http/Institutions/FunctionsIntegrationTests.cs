using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Functions.Http.Institutions;

/// <summary>
///     Integration tests for the Azure Functions backend using shared Mockoon fixture.
/// </summary>
public class FunctionsIntegrationTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    /// <summary>
    ///     Tests that the GetInstitutions endpoint returns OK status code and valid JSON response.
    /// </summary>
    [Fact]
    public async Task GetInstitutionsEndpointReturnsOkStatusCodeAndValidJson()
    {
        // Act
        var institutions = await Client.GetInstitutionsAsync();

        // Assert
        Assert.NotNull(institutions);

        var firstInstitution = institutions.First();
        Assert.NotNull(firstInstitution.Id);
        Assert.NotNull(firstInstitution.Name);
        Assert.Contains(Constants.SandboxInstitutionId, firstInstitution.Id);
    }
}
