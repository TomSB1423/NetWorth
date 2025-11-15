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
    IAccountRepository accountRepository,
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
            "Syncing account and transactions for account {AccountId}, user {UserId}",
            request.AccountId,
            request.UserId);

        // First, fetch and cache account metadata
        var account = await financialProvider.GetAccountAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            logger.LogError("Account {AccountId} not found during sync", request.AccountId);
            return new SyncAccountCommandResult
            {
                AccountId = request.AccountId,
                TransactionCount = 0,
                DateFrom = request.DateFrom ?? DateTimeOffset.UtcNow.AddDays(-90),
                DateTo = request.DateTo ?? DateTimeOffset.UtcNow
            };
        }

        // Fetch account details for additional metadata
        var accountDetails = await financialProvider.GetAccountDetailsAsync(request.AccountId, cancellationToken);

        if (accountDetails != null)
        {
            // We need requisitionId to save account. Since we don't have it in the command,
            // we'll need to look it up from existing account or skip this for now.
            // For now, let's fetch it from the existing account if it exists.
            var existingAccounts = await accountRepository.GetAccountsByUserIdAsync(request.UserId, cancellationToken);
            var existingAccount = existingAccounts.FirstOrDefault(a => a.Id == request.AccountId);

            if (existingAccount != null)
            {
                // Account exists, update with latest details
                await accountRepository.UpsertAccountAsync(
                    request.AccountId,
                    request.UserId,
                    existingAccount.RequisitionId,
                    account.InstitutionId,
                    accountDetails,
                    cancellationToken);

                logger.LogInformation("Updated account {AccountId} metadata", request.AccountId);
            }
            else
            {
                logger.LogWarning(
                    "Account {AccountId} not found in database, cannot save without requisitionId. Skipping account save.",
                    request.AccountId);
            }
        }

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
            logger.LogError("Failed to retrieve transactions for account {AccountId}", request.AccountId);
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
