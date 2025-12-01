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
            "Syncing account {AccountId} for user {UserId}",
            request.AccountId,
            request.UserId);
        var account = await financialProvider.GetAccountAsync(request.AccountId, cancellationToken);

        if (account is null)
        {
            logger.LogError(
                "Account {AccountId} not found during sync - may have been deleted or access revoked",
                request.AccountId);
            return new SyncAccountCommandResult
            {
                AccountId = request.AccountId,
                TransactionCount = 0,
                DateFrom = request.DateFrom ?? DateTimeOffset.UtcNow.AddDays(-90),
                DateTo = request.DateTo ?? DateTimeOffset.UtcNow
            };
        }

        logger.LogDebug(
            "Account metadata: Institution={InstitutionId}, Status={Status}, Name={Name}",
            account.InstitutionId,
            account.Status,
            account.Name);
        var accountDetails = await financialProvider.GetAccountDetailsAsync(request.AccountId, cancellationToken);

        if (accountDetails != null)
        {
            logger.LogDebug(
                "Account details: Currency={Currency}, Type={Type}, Product={Product}",
                accountDetails.Currency,
                accountDetails.CashAccountType,
                accountDetails.Product);
            var existingAccounts = await accountRepository.GetAccountsByUserIdAsync(request.UserId, cancellationToken);
            var existingAccount = existingAccounts.FirstOrDefault(a => a.Id == request.AccountId);

            if (existingAccount != null)
            {
                await accountRepository.UpsertAccountAsync(
                    request.AccountId,
                    request.UserId,
                    existingAccount.RequisitionId,
                    account.InstitutionId,
                    accountDetails,
                    cancellationToken);
            }
            else
            {
                logger.LogWarning(
                    "Account {AccountId} not found in database, cannot save without requisitionId. Skipping account metadata save.",
                    request.AccountId);
            }
        }
        else
        {
            logger.LogDebug("Account details not available for {AccountId}", request.AccountId);
        }

        var dateTo = request.DateTo ?? DateTimeOffset.UtcNow;
        var dateFrom = request.DateFrom ?? dateTo.AddDays(-90);

        logger.LogDebug(
            "Fetching transactions from {DateFrom} to {DateTo}",
            dateFrom.ToString("yyyy-MM-dd"),
            dateTo.ToString("yyyy-MM-dd"));
        var transactions = await financialProvider.GetAccountTransactionsAsync(
            request.AccountId,
            dateFrom,
            dateTo,
            cancellationToken);

        if (transactions is null)
        {
            logger.LogError(
                "Failed to retrieve transactions for account {AccountId} - account may have been deleted",
                request.AccountId);
            return new SyncAccountCommandResult
            {
                AccountId = request.AccountId,
                TransactionCount = 0,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }

        var transactionList = transactions.ToList();

        logger.LogDebug(
            "Retrieved {Count} transactions for account {AccountId}",
            transactionList.Count,
            request.AccountId);
        await transactionRepository.UpsertTransactionsAsync(
            request.AccountId,
            request.UserId,
            transactionList,
            cancellationToken);

        logger.LogInformation(
            "Synced {Count} transactions for account {AccountId}",
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
