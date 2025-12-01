using System.Text.Json;
using Aspire.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Networth.Infrastructure.Data.Context;
using Networth.ServiceDefaults;
using Npgsql;
using Projects;

namespace Networth.SystemTests;

/// <summary>
///     System tests for the institution synchronization workflow.
///     These tests spin up the complete Aspire application with all real services
///     (PostgreSQL, Azure Storage, Functions) and interact with the real GoCardless sandbox API.
/// </summary>
public class InstitutionSyncSystemTests
{
    private const string SandboxInstitutionId = "SANDBOXFINANCE_SFIN0000";

    /// <summary>
    ///     System test that verifies the complete sync workflow for SANDBOXFINANCE institution.
    ///     This test:
    ///     1. Starts the full Aspire application with PostgreSQL and Azure Storage
    ///     2. Calls the sync institution endpoint using real GoCardless sandbox API
    ///     3. Waits for queue processing to complete
    ///     4. Verifies accounts and requisitions are saved to the database
    ///
    ///     Note: This requires valid GoCardless sandbox credentials to be configured.
    /// </summary>
    [Fact]
    public async Task SyncInstitution_WithSandboxFinance_EnqueuesAccountsForSync()
    {
        // Arrange
        await using var app = await CreateDistributedApplicationAsync();

        string connectionString = await GetDatabaseConnectionStringAsync(app);

        HttpClient functionsClient = app.CreateHttpClient(ResourceNames.Functions);
        var client = new NetworthClient(functionsClient);

        // Act
        var result = await client.SyncInstitutionAsync(SandboxInstitutionId);

        // Assert - Verify API response
        Assert.Equal(SandboxInstitutionId, result.InstitutionId);
        Assert.True(result.AccountsEnqueued > 0, "Should enqueue at least one account for sync");
        Assert.Equal(result.AccountsEnqueued, result.AccountIds.Count);
        Assert.All(result.AccountIds, accountId => Assert.False(string.IsNullOrWhiteSpace(accountId)));
        // Assert - Verify API response
        Assert.Equal(SandboxInstitutionId, result.InstitutionId);
        Assert.True(result.AccountsEnqueued > 0, "Should enqueue at least one account for sync");
        Assert.Equal(result.AccountsEnqueued, result.AccountIds.Count);
        Assert.All(result.AccountIds, accountId => Assert.False(string.IsNullOrWhiteSpace(accountId)));

        // Wait a bit for database writes to complete
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Assert - Verify database changes
        await using var dbContext = CreateDbContext(connectionString);

        // Verify requisition was updated/created
        var requisitions = await dbContext.Requisitions
            .Where(r => r.InstitutionId == SandboxInstitutionId)
            .ToListAsync();

        Assert.NotEmpty(requisitions);

        var requisition = requisitions.First();
        Assert.NotNull(requisition);
        Assert.Equal(SandboxInstitutionId, requisition.InstitutionId);

        // Verify accounts were created in database
        var accounts = await dbContext.Accounts
            .Where(a => a.InstitutionId == SandboxInstitutionId)
            .ToListAsync();

        Assert.NotEmpty(accounts);
        Assert.Equal(result.AccountsEnqueued, accounts.Count);

        // Verify each account from the response exists in the database
        foreach (string accountId in result.AccountIds)
        {
            var dbAccount = accounts.FirstOrDefault(a => a.Id == accountId);
            Assert.NotNull(dbAccount);
            Assert.Equal(SandboxInstitutionId, dbAccount.InstitutionId);
            Assert.Equal(requisition.Id, dbAccount.RequisitionId);
            Assert.NotNull(dbAccount.Name);
            Assert.NotNull(dbAccount.Currency);
        }
    }

    /// <summary>
    ///     Helper test to verify we can retrieve institutions from the GoCardless sandbox API.
    ///     This ensures the application is properly configured and can communicate with GoCardless.
    /// </summary>
    [Fact]
    public async Task GetInstitutions_ReturnsInstitutionsFromGoCardlessSandbox()
    {
        // Arrange
        await using var app = await CreateDistributedApplicationAsync();
        HttpClient functionsClient = app.CreateHttpClient(ResourceNames.Functions);
        var client = new NetworthClient(functionsClient);

        // Act
        var institutions = await client.GetInstitutionsAsync();

        // Assert
        Assert.NotEmpty(institutions);
        Assert.Contains(institutions, i => i.Id == SandboxInstitutionId);
    }

    /// <summary>
    ///     Creates and starts a distributed application for system testing.
    ///     This includes PostgreSQL, Azure Storage emulator, and the Functions service.
    ///     Uses real GoCardless sandbox API (no mocking).
    /// </summary>
    private static async Task<DistributedApplication> CreateDistributedApplicationAsync()
    {
        IDistributedApplicationTestingBuilder appBuilder =
            await DistributedApplicationTestingBuilder.CreateAsync<Networth_AppHost>(
                [],
                (appOptions, _) =>
                {
                    appOptions.DisableDashboard = true;
                });

        // Build and start the application
        DistributedApplication app = await appBuilder.BuildAsync();
        await app.StartAsync();

        return app;
    }

    /// <summary>
    ///     Gets the PostgreSQL database connection string from the running application.
    /// </summary>
    private static async Task<string> GetDatabaseConnectionStringAsync(DistributedApplication app)
    {
        // Get the connection string from the networth-db resource
        var connectionString = await app.GetConnectionStringAsync(ResourceNames.NetworthDb);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string not found");
        }

        return connectionString;
    }

    /// <summary>
    ///     Creates a DbContext for direct database access in tests.
    /// </summary>
    private static NetworthDbContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NetworthDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new NetworthDbContext(optionsBuilder.Options);
    }
}
