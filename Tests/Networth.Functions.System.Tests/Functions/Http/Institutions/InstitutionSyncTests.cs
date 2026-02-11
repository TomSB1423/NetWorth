using Microsoft.EntityFrameworkCore;
using Networth.Domain.Enums;
using Networth.Functions.Models.Responses;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.SystemTests.Helpers;
using Networth.SystemTests.Infrastructure;
using Xunit.Abstractions;

namespace Networth.SystemTests.Functions.Http.Institutions;

public class InstitutionSyncTests(ITestOutputHelper testOutput)
    : SystemTestBase(testOutput)
{
    /// <summary>
    ///     System test that verifies the complete sync workflow for SANDBOXFINANCE institution.
    ///     This test:
    ///     1. Starts the full Aspire application with PostgreSQL and Azure Storage
    ///     2. Links and authorizes the GoCardless sandbox test institution
    ///     3. Syncs the institution to fetch account details
    ///     4. Waits for queue processing to complete
    ///     5. Verifies accounts and requisitions are saved to the database
    ///     Note: Uses the GoCardless sandbox institution for testing with real API credentials.
    /// </summary>
    [Fact]
    public async Task SyncInstitution_WithSandboxFinance_EnqueuesAccountsForSync()
    {
        LinkInstitutionResponse linkResponse = await Client.LinkInstitutionAsync(Constants.SandboxInstitutionId);
        Assert.NotNull(linkResponse.AuthorizationLink);

        await using GoCardlessSandboxAuthorizer authorizer = new();
        await authorizer.AuthorizeRequisitionAsync(linkResponse.AuthorizationLink);

        // Act
        await Client.SyncInstitutionAsync(Constants.SandboxInstitutionId);

        // Wait for queue processing to complete
        await WaitForLog($"Successfully processed institution sync for institution {Constants.SandboxInstitutionId}");

        // Assert - Verify database changes
        await using NetworthDbContext dbContext = CreateDbContext();

        List<Requisition> requisitions = await dbContext.Requisitions
            .Where(r => r.InstitutionId == Constants.SandboxInstitutionId)
            .ToListAsync();
        Assert.NotEmpty(requisitions);

        Requisition requisition = requisitions.First();
        Assert.NotNull(requisition);
        Assert.Equal(Constants.SandboxInstitutionId, requisition.InstitutionId);

        // Verify accounts were created in database
        List<Account> accounts = await dbContext.Accounts
            .Where(a => a.InstitutionId == Constants.SandboxInstitutionId)
            .ToListAsync();
        Assert.NotEmpty(accounts);

        // Verify each account exists in the database
        foreach (Account dbAccount in accounts)
        {
            Assert.Equal(Constants.SandboxInstitutionId, dbAccount.InstitutionId);
            Assert.Equal(requisition.Id, dbAccount.RequisitionId);
            Assert.NotNull(dbAccount.Name);
            Assert.NotNull(dbAccount.Currency);
        }
    }

    /// <summary>
    ///     System test to verify we can retrieve institutions from the GoCardless API.
    ///     This ensures the application is properly configured and can communicate with GoCardless.
    /// </summary>
    [Fact]
    public async Task GetInstitutions_ReturnsInstitutionsFromGoCardless()
    {
        // Act
        List<InstitutionResponse> institutions = await Client.GetInstitutionsAsync();

        // Assert - Verify we got a list of real UK institutions
        Assert.NotEmpty(institutions);
        Assert.True(institutions.Count > 10, "Expected at least 10 institutions from GoCardless");

        // Verify institutions have required properties
        foreach (var institution in institutions.Take(5))
        {
            Assert.NotNull(institution.Id);
            Assert.NotNull(institution.Name);
        }
    }

    /// <summary>
    ///     System test that verifies the complete flow of linking a bank account and then syncing it.
    ///     This test:
    ///     1. Links a bank account to the GoCardless sandbox institution
    ///     2. Verifies the requisition is created in the database
    ///     3. Authorizes the requisition via GoCardless OAuth flow (using Playwright)
    ///     4. Syncs the institution to fetch account details
    ///     5. Verifies accounts are created and enqueued for transaction sync
    ///     Note: Uses real GoCardless API credentials with their sandbox test institution.
    /// </summary>
    [Fact]
    public async Task LinkAccount_ThenSyncInstitution_CreatesAccountsInDatabase()
    {
        LinkInstitutionResponse linkResult = await Client.LinkInstitutionAsync(Constants.SandboxInstitutionId);

        Assert.NotNull(linkResult);
        Assert.NotNull(linkResult.AuthorizationLink);
        Assert.NotEmpty(linkResult.AuthorizationLink);
        Assert.Equal(AccountLinkStatus.Pending, linkResult.Status);

        // Verify requisition was created in database
        await using NetworthDbContext dbContext1 = CreateDbContext();
        List<Requisition> requisitions = await dbContext1.Requisitions
            .Where(r => r.InstitutionId == Constants.SandboxInstitutionId)
            .ToListAsync();

        Assert.NotEmpty(requisitions);
        Requisition requisition = requisitions.First();
        Assert.NotNull(requisition);
        Assert.Equal(Constants.SandboxInstitutionId, requisition.InstitutionId);

        // Act - Step 2: Authorize the requisition via GoCardless OAuth flow
        await using GoCardlessSandboxAuthorizer authorizer = new();
        await authorizer.AuthorizeRequisitionAsync(linkResult.AuthorizationLink);

        // Wait for OAuth flow to complete and GoCardless to update the requisition
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Act - Sync Institution
        await Client.SyncInstitutionAsync(Constants.SandboxInstitutionId);

        // Wait for accounts to be created in database
        await WaitFor(
            async () =>
            {
                await using NetworthDbContext dbContext = CreateDbContext();
                return await dbContext.Accounts.AnyAsync(a => a.InstitutionId == Constants.SandboxInstitutionId);
            },
            failureMessage: "Accounts were not created within timeout.");

        // Verify accounts were created in database
        await using NetworthDbContext dbContext2 = CreateDbContext();
        List<Account> accounts = await dbContext2.Accounts
            .Where(a => a.InstitutionId == Constants.SandboxInstitutionId)
            .ToListAsync();

        Assert.NotEmpty(accounts);

        // Verify each account from the sync response exists in the database
        foreach (Account dbAccount in accounts)
        {
            Assert.Equal(Constants.SandboxInstitutionId, dbAccount.InstitutionId);
            Assert.NotNull(dbAccount.RequisitionId); // Just verify it has a requisition
            Assert.NotNull(dbAccount.Name);
            Assert.NotNull(dbAccount.Currency);
        }

        List<string> accountIds = [.. accounts.Select(a => a.Id)];

        // Wait for transactions to be synced
        await WaitFor(
            async () =>
            {
                await using NetworthDbContext dbContext = CreateDbContext();
                return await dbContext.Transactions.AnyAsync(t => accountIds.Contains(t.AccountId));
            },
            failureMessage: "Transactions were not synced within timeout.");

        List<Transaction> transactions = await dbContext2.Transactions
            .Where(t => accountIds.Contains(t.AccountId))
            .ToListAsync();

        // Verify transactions have required fields populated
        foreach (Transaction transaction in transactions)
        {
            Assert.NotNull(transaction.Id);
            Assert.NotNull(transaction.TransactionId);
            Assert.NotNull(transaction.AccountId);
            Assert.Equal(Networth.Functions.Tests.Integration.Constants.MockUserId, transaction.UserId);
            Assert.NotNull(transaction.Currency);
            Assert.NotEqual(0, transaction.Amount);
            Assert.True(transaction.ImportedAt > DateTime.MinValue);
        }

        // Verify at least one account has transactions
        List<string> accountsWithTransactions = transactions.Select(t => t.AccountId).Distinct().ToList();
        Assert.NotEmpty(accountsWithTransactions);
    }
}
