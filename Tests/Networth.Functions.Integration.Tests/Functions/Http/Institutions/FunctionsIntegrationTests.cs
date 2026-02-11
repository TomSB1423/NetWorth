using Microsoft.EntityFrameworkCore;
using Networth.Functions.Tests.Integration.Fixtures;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Configurations;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.ServiceDefaults;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Functions.Http.Institutions;

/// <summary>
///     Integration tests for the Azure Functions backend using shared Mockoon fixture.
/// </summary>
[Collection("Integration")]
public class FunctionsIntegrationTests(MockoonTestFixture mockoonTestFixture, ITestOutputHelper testOutput)
    : IntegrationTestBase(mockoonTestFixture, testOutput)
{
    /// <summary>
    ///     Tests that the GetInstitutions endpoint returns OK status code and valid JSON response.
    /// </summary>
    [Fact]
    public async Task GetInstitutionsEndpointReturnsOkStatusCodeAndValidJson()
    {
        // Ensure database is created with migrations
        var dbConnectionString = await App.GetConnectionStringAsync(ResourceNames.NetworthDb);
        Assert.False(string.IsNullOrEmpty(dbConnectionString), "Connection string not found");

        var services = new ServiceCollection();
        services.AddDbContext<NetworthDbContext>(options => options.UseNpgsql(dbConnectionString));
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        // Create database schema (uses Institutions table by default)
        await dbContext.Database.EnsureCreatedAsync();

        // Manually create SandboxInstitution table if it doesn't exist
        // This is needed because Functions app uses sandbox mode
        await dbContext.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "SandboxInstitution" (
                id character varying(100) NOT NULL,
                country_code character varying(2) NOT NULL,
                name character varying(200) NOT NULL,
                logo_url character varying(500),
                bic character varying(50),
                countries jsonb NOT NULL,
                supported_features jsonb,
                last_updated timestamp with time zone NOT NULL,
                PRIMARY KEY (id, country_code)
            )
            """);

        // Seed sandbox institution data
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO "SandboxInstitution" (id, country_code, name, logo_url, bic, countries, supported_features, last_updated)
            VALUES (
                'SANDBOXFINANCE_SFIN0000',
                'GB',
                'Sandbox Finance',
                'https://cdn.nordigen.com/ais/SANDBOXFINANCE_SFIN0000.png',
                'SFIN0000',
                '["GB"]',
                '["account_selection", "business_accounts"]',
                NOW()
            )
            ON CONFLICT DO NOTHING
            """);

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
