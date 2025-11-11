using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for syncing account transactions from GoCardless to the database.
/// </summary>
public class SyncAccountCommandHandler(
    IFinancialProvider financialProvider,
    ITransactionRepository transactionRepository,
    ILogger<SyncAccountCommandHandler> logger)
    : IRequestHandler<SyncAccountCommand, SyncAccountCommandResult>
{
    /// <inheritdoc />
    public async Task<SyncAccountCommandResult> HandleAsync(
        SyncAccountCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Syncing transactions for account {AccountId}, user {UserId}",
            request.AccountId,
            request.UserId);

        // Determine date range (default to last 90 days if not specified)
        var dateTo = request.DateTo ?? DateTimeOffset.UtcNow;
        var dateFrom = request.DateFrom ?? dateTo.AddDays(-90);

        // Fetch transactions from GoCardless
        var transactions = await financialProvider.GetAccountTransactionsAsync(
            request.AccountId,
            dateFrom,
            dateTo,
            cancellationToken);

        // If transactions is null, account was not found - return empty result
        if (transactions is null)
        {
            logger.LogError("Account {AccountId} not found during sync", request.AccountId);
            return new SyncAccountCommandResult
            {
                AccountId = request.AccountId,
                TransactionCount = 0,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }

        var transactionList = transactions.ToList();

        logger.LogInformation(
            "Retrieved {Count} transactions from GoCardless for account {AccountId}",
            transactionList.Count,
            request.AccountId);

        // Upsert transactions to database
        await transactionRepository.UpsertTransactionsAsync(
            request.AccountId,
            request.UserId,
            transactionList,
            cancellationToken);

        logger.LogInformation(
            "Successfully synced {Count} transactions for account {AccountId}",
            transactionList.Count,
            request.AccountId);

        return new SyncAccountCommandResult
        {
            AccountId = request.AccountId,
            TransactionCount = transactionList.Count,
            DateFrom = dateFrom,
            DateTo = dateTo
        };
    }
}
