using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Enums;
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
            "Syncing account {AccountId} for user {UserId}",
            request.AccountId,
            request.UserId);

        await accountRepository.UpdateAccountStatusAsync(request.AccountId, AccountLinkStatus.Syncing, cancellationToken);

        if (!await SyncAccountMetadataAsync(request.AccountId, request.UserId, cancellationToken))
        {
            return new SyncAccountCommandResult
            {
                AccountId = request.AccountId,
                TransactionCount = 0,
                DateFrom = request.DateFrom ?? DateTimeOffset.UtcNow.AddDays(-90),
                DateTo = request.DateTo ?? DateTimeOffset.UtcNow
            };
        }

        await SyncAccountBalancesAsync(request.AccountId, cancellationToken);

        var dateTo = request.DateTo ?? DateTimeOffset.UtcNow;
        var dateFrom = request.DateFrom ?? dateTo.AddDays(-90);

        var transactionCount = await SyncAccountTransactionsAsync(request.AccountId, request.UserId, dateFrom, dateTo, cancellationToken);

        if (transactionCount == null)
        {
            return new SyncAccountCommandResult
            {
                AccountId = request.AccountId,
                TransactionCount = 0,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }

        await accountRepository.UpdateAccountStatusAsync(request.AccountId, AccountLinkStatus.Linked, cancellationToken);

        return new SyncAccountCommandResult
        {
            AccountId = request.AccountId,
            TransactionCount = transactionCount.Value,
            DateFrom = dateFrom,
            DateTo = dateTo
        };
    }

    private async Task<bool> SyncAccountMetadataAsync(string accountId, string userId, CancellationToken cancellationToken)
    {
        var account = await financialProvider.GetAccountAsync(accountId, cancellationToken);

        if (account is null)
        {
            logger.LogError(
                "Account {AccountId} not found during sync - may have been deleted or access revoked",
                accountId);
            return false;
        }

        logger.LogDebug(
            "Account metadata: Institution={InstitutionId}, Status={Status}, Name={Name}",
            account.InstitutionId,
            account.Status,
            account.Name);
        var accountDetails = await financialProvider.GetAccountDetailsAsync(accountId, cancellationToken);

        if (accountDetails != null)
        {
            logger.LogDebug(
                "Account details: Currency={Currency}, Type={Type}, Product={Product}",
                accountDetails.Currency,
                accountDetails.CashAccountType,
                accountDetails.Product);
            var existingAccounts = await accountRepository.GetAccountsByUserIdAsync(userId, cancellationToken);
            var existingAccount = existingAccounts.FirstOrDefault(a => a.Id == accountId);

            if (existingAccount != null)
            {
                await accountRepository.UpsertAccountAsync(
                    accountId,
                    userId,
                    existingAccount.RequisitionId,
                    account.InstitutionId,
                    accountDetails,
                    cancellationToken);
            }
            else
            {
                logger.LogWarning(
                    "Account {AccountId} not found in database, cannot save without requisitionId. Skipping account metadata save.",
                    accountId);
            }
        }
        else
        {
            logger.LogDebug("Account details not available for {AccountId}", accountId);
        }

        return true;
    }

    private async Task SyncAccountBalancesAsync(string accountId, CancellationToken cancellationToken)
    {
        var balances = await financialProvider.GetAccountBalancesAsync(accountId, cancellationToken);
        if (balances != null)
        {
            await accountRepository.UpsertAccountBalancesAsync(accountId, balances, cancellationToken);
        }
        else
        {
            logger.LogWarning("Balances not available for account {AccountId}", accountId);
        }
    }

    private async Task<int?> SyncAccountTransactionsAsync(
        string accountId,
        string userId,
        DateTimeOffset dateFrom,
        DateTimeOffset dateTo,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Fetching transactions from {DateFrom} to {DateTo}",
            dateFrom.ToString("yyyy-MM-dd"),
            dateTo.ToString("yyyy-MM-dd"));
        var transactions = await financialProvider.GetAccountTransactionsAsync(
            accountId,
            dateFrom,
            dateTo,
            cancellationToken);

        if (transactions is null)
        {
            logger.LogError(
                "Failed to retrieve transactions for account {AccountId} - account may have been deleted",
                accountId);
            return null;
        }

        var transactionList = transactions.ToList();

        logger.LogDebug(
            "Retrieved {Count} transactions for account {AccountId}",
            transactionList.Count,
            accountId);
        await transactionRepository.UpsertTransactionsAsync(
            accountId,
            userId,
            transactionList,
            cancellationToken);

        logger.LogInformation(
            "Synced {Count} transactions for account {AccountId}",
            transactionList.Count,
            accountId);

        return transactionList.Count;
    }
}
