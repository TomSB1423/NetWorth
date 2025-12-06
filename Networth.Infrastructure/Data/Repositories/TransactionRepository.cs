using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using DomainTransaction = Networth.Domain.Entities.Transaction;
using InfraTransaction = Networth.Infrastructure.Data.Entities.Transaction;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for Transaction entities.
/// </summary>
public class TransactionRepository(NetworthDbContext context, ILogger<TransactionRepository> logger)
    : ITransactionRepository
{
    /// <inheritdoc />
    public async Task UpsertTransactionsAsync(
        string accountId,
        string userId,
        IEnumerable<DomainTransaction> transactions,
        CancellationToken cancellationToken = default)
    {
        var transactionList = transactions.ToList();

        logger.LogInformation(
            "Upserting {Count} transactions for account {AccountId}",
            transactionList.Count,
            accountId);

        var transactionIds = transactionList.Select(t => t.Id).ToList();

        // Get existing transactions
        var existingTransactions = await context.Transactions
            .Where(t => t.AccountId == accountId && transactionIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        var existingIds = existingTransactions.Select(t => t.Id).ToHashSet();

        // Map domain to infrastructure entities
        var infraTransactions = transactionList.Select(dt => new InfraTransaction
        {
            Id = dt.Id,
            UserId = userId,
            AccountId = dt.AccountId,
            TransactionId = dt.TransactionId ?? dt.Id,
            DebtorName = dt.DebtorName,
            DebtorAccountIban = dt.DebtorAccount,
            Amount = dt.Amount,
            Currency = dt.Currency,
            BankTransactionCode = dt.BankTransactionCode,
            BookingDate = dt.BookingDate,
            ValueDate = dt.ValueDate,
            RemittanceInformationUnstructured = dt.RemittanceInformationUnstructured,
            Status = dt.Status,
        }).ToList();

        // Separate new and existing
        var newTransactions = infraTransactions.Where(t => !existingIds.Contains(t.Id)).ToList();
        var updatedTransactions = infraTransactions.Where(t => existingIds.Contains(t.Id)).ToList();

        // Add new transactions
        if (newTransactions.Any())
        {
            await context.Transactions.AddRangeAsync(newTransactions, cancellationToken);
            logger.LogInformation(
                "Added {Count} new transactions for account {AccountId}",
                newTransactions.Count,
                accountId);
        }

        // Update existing transactions
        if (updatedTransactions.Any())
        {
            context.Transactions.UpdateRange(updatedTransactions);
            logger.LogInformation(
                "Updated {Count} existing transactions for account {AccountId}",
                updatedTransactions.Count,
                accountId);
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Successfully upserted {Count} transactions for account {AccountId}",
            transactionList.Count,
            accountId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DomainTransaction>> GetByAccountIdAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving transactions for account {AccountId}", accountId);

        var transactions = await context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.BookingDate ?? t.ValueDate ?? t.ImportedAt)
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Successfully retrieved {Count} transactions for account {AccountId}",
            transactions.Count,
            accountId);

        // Map infrastructure to domain entities
        return transactions.Select(it => new DomainTransaction
        {
            Id = it.Id,
            AccountId = it.AccountId,
            TransactionId = it.TransactionId,
            DebtorName = it.DebtorName,
            DebtorAccount = it.DebtorAccountIban,
            Amount = it.Amount,
            Currency = it.Currency,
            BankTransactionCode = it.BankTransactionCode,
            BookingDate = it.BookingDate,
            ValueDate = it.ValueDate,
            RemittanceInformationUnstructured = it.RemittanceInformationUnstructured,
            Status = it.Status,
        });
    }

    /// <inheritdoc />
    public async Task<int> CalculateRunningBalancesAsync(string accountId, CancellationToken cancellationToken = default)
    {
        // 1. Fetch balances to determine the starting point (latest balance)
        var balances = await context.AccountBalances
            .Where(b => b.AccountId == accountId)
            .ToListAsync(cancellationToken);

        if (balances.Count == 0)
        {
            logger.LogWarning("No balance found for account {AccountId}", accountId);
            return 0;
        }

        // Try to find closingBooked balance first, then fall back to any balance
        var latestBalance = balances
            .Where(b => b.BalanceType == "closingBooked")
            .OrderByDescending(b => b.ReferenceDate)
            .FirstOrDefault()
            ?? balances
            .OrderByDescending(b => b.ReferenceDate)
            .FirstOrDefault();

        if (latestBalance == null)
        {
            logger.LogWarning("No valid balance found for account {AccountId}", accountId);
            return 0;
        }

        logger.LogInformation(
            "Calculating running balances for account {AccountId} starting from balance {Balance} ({Type}) at {Date}",
            accountId,
            latestBalance.Amount,
            latestBalance.BalanceType,
            latestBalance.ReferenceDate);

        // 2. Perform the update using raw SQL for performance.
        // We use a CTE with a window function to calculate the cumulative sum of amounts
        // for all transactions preceding (in the sort order) the current one.
        //
        // Sort order: BookingDate DESC, TransactionId DESC (Newest to Oldest)
        //
        // Logic:
        // RunningBalance(T_current) = StartBalance - Sum(Amounts of transactions NEWER than T_current)
        //
        // The window function `SUM("Amount") OVER (...)` with `ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING`
        // calculates exactly that sum of newer transactions.
        //
        // Note: We use NULLS LAST for BookingDate to match C# OrderByDescending behavior for nulls.

        int count = await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            WITH RunningTotals AS (
                SELECT
                    "Id",
                    SUM("Amount") OVER (
                        PARTITION BY "AccountId"
                        ORDER BY "BookingDate" DESC NULLS LAST, "TransactionId" DESC
                        ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING
                    ) as "AmountToSubtract"
                FROM "Transactions"
                WHERE "AccountId" = {accountId}
            )
            UPDATE "Transactions" t
            SET "RunningBalance" = {latestBalance.Amount} - COALESCE(rt."AmountToSubtract", 0)
            FROM RunningTotals rt
            WHERE t."Id" = rt."Id"
            """,
            cancellationToken);

        logger.LogInformation("Updated running balance for {Count} transactions", count);

        return count;
    }
}
