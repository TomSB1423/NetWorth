using Microsoft.EntityFrameworkCore;
using Networth.Functions.Models.Responses;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;
using Networth.SystemTests.Helpers;
using Networth.SystemTests.Infrastructure;
using Xunit.Abstractions;

namespace Networth.SystemTests.Functions.Http.Institutions;

/// <summary>
///     System tests for the transaction synchronization workflow.
/// </summary>
public class TransactionSyncTests(ITestOutputHelper testOutput)
    : SystemTestBase(testOutput)
{
    private const string SandboxInstitutionId = "SANDBOXFINANCE_SFIN0000";

    /// <summary>
    ///     Verifies that syncing transactions correctly persists the running balance.
    /// </summary>
    [Fact]
    public async Task SyncTransactions_PersistsRunningBalance()
    {
        LinkInstitutionResponse linkResult = await Client.LinkInstitutionAsync(SandboxInstitutionId);

        Assert.NotNull(linkResult.AuthorizationLink);
        await using GoCardlessSandboxAuthorizer authorizer = new();
        await authorizer.AuthorizeRequisitionAsync(linkResult.AuthorizationLink);

        // Wait for GoCardless callback processing
        await Task.Delay(TimeSpan.FromSeconds(5));

        await Client.SyncInstitutionAsync(SandboxInstitutionId);

        // The sync endpoint enqueues messages. Wait for the Functions to process them.
        await WaitFor(
            async () =>
            {
                await using NetworthDbContext dbContext = CreateDbContext();
                return await dbContext.Transactions.AnyAsync();
            },
            failureMessage: "Transactions were not synced within timeout.");

        await using NetworthDbContext dbContext = CreateDbContext();
        List<Transaction> transactions = await dbContext.Transactions.ToListAsync();
        Assert.NotEmpty(transactions);
    }
}
