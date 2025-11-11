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
            OwnerId = userId,
            AccountId = dt.AccountId,
            Value = dt.Amount,
            Currency = dt.Currency,
            Time = dt.BookingDate ?? dt.ValueDate ?? DateTime.UtcNow
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
            .OrderByDescending(t => t.Time)
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
            Amount = it.Value,
            Currency = it.Currency,
            BookingDate = it.Time,
            ValueDate = it.Time
        });
    }
}
