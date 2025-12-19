using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Queries;
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
        var transactionList = transactions.DistinctBy(t => t.Id).ToList();

        logger.LogInformation(
            "Upserting {Count} transactions for account {AccountId}",
            transactionList.Count,
            accountId);

        var transactionIds = transactionList.Select(t => t.Id).ToList();

        // Get existing transactions with tracking
        var existingTransactions = await context.Transactions
            .Where(t => t.AccountId == accountId && transactionIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

        var newTransactions = new List<InfraTransaction>();

        foreach (var dt in transactionList)
        {
            if (existingTransactions.TryGetValue(dt.Id, out var existingTransaction))
            {
                // Update existing transaction
                existingTransaction.TransactionId = dt.TransactionId ?? dt.Id;
                existingTransaction.DebtorName = dt.DebtorName;
                existingTransaction.DebtorAccountIban = dt.DebtorAccount;
                existingTransaction.Amount = dt.Amount;
                existingTransaction.Currency = dt.Currency;
                existingTransaction.BankTransactionCode = dt.BankTransactionCode;
                existingTransaction.BookingDate = dt.BookingDate;
                existingTransaction.ValueDate = dt.ValueDate;
                existingTransaction.RemittanceInformationUnstructured = dt.RemittanceInformationUnstructured;
                existingTransaction.Status = dt.Status;
                existingTransaction.RunningBalance = dt.BalanceAfterTransaction;
            }
            else
            {
                // Create new transaction
                newTransactions.Add(new InfraTransaction
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
                    RunningBalance = dt.BalanceAfterTransaction,
                });
            }
        }

        if (newTransactions.Any())
        {
            await context.Transactions.AddRangeAsync(newTransactions, cancellationToken);
            logger.LogInformation(
                "Added {Count} new transactions for account {AccountId}",
                newTransactions.Count,
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
    public async Task<(IEnumerable<DomainTransaction> Items, int TotalCount)> GetByAccountIdPagedAsync(
        string accountId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Retrieving paginated transactions for account {AccountId}, page {Page}, pageSize {PageSize}",
            accountId,
            page,
            pageSize);

        var query = context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.BookingDate ?? t.ValueDate ?? t.ImportedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var transactions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Successfully retrieved {Count} of {Total} transactions for account {AccountId} (page {Page})",
            transactions.Count,
            totalCount,
            accountId,
            page);

        // Map infrastructure to domain entities
        var items = transactions.Select(it => new DomainTransaction
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

        return (items, totalCount);
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
        // Note: We use ExecuteSqlRawAsync instead of ExecuteSqlInterpolatedAsync because we want to use
        // nameof() for table/column names (which should be literals) but still parameterize values.

        var sql = $$"""
            WITH RunningTotals AS (
                SELECT
                    "{{nameof(InfraTransaction.Id)}}",
                    SUM("{{nameof(InfraTransaction.Amount)}}") OVER (
                        PARTITION BY "{{nameof(InfraTransaction.AccountId)}}"
                        ORDER BY "{{nameof(InfraTransaction.BookingDate)}}" DESC NULLS LAST, "{{nameof(InfraTransaction.TransactionId)}}" DESC
                        ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING
                    ) as "AmountToSubtract"
                FROM "{{nameof(context.Transactions)}}"
                WHERE "{{nameof(InfraTransaction.AccountId)}}" = {0}
            )
            UPDATE "{{nameof(context.Transactions)}}" t
            SET "{{nameof(InfraTransaction.RunningBalance)}}" = {1} - COALESCE(rt."AmountToSubtract", 0)
            FROM RunningTotals rt
            WHERE t."{{nameof(InfraTransaction.Id)}}" = rt."{{nameof(InfraTransaction.Id)}}"
            """;

        int count = await context.Database.ExecuteSqlRawAsync(
            sql,
            new object[] { accountId, latestBalance.Amount },
            cancellationToken);

        logger.LogInformation("Updated running balance for {Count} transactions", count);

        return count;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Networth.Domain.Entities.NetWorthPoint>> GetNetWorthHistoryAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var sql = NetWorthHistoryQuery.GetFullQuery("{0}");

        return await context.Database.SqlQueryRaw<Networth.Domain.Entities.NetWorthPoint>(
            sql,
            userId)
            .ToListAsync(cancellationToken);
    }
}
