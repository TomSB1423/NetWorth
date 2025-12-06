using Networth.Domain.Entities;

namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for Transaction entities.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    ///     Adds or updates multiple transactions for a specific account.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="userId">The user ID who owns the transactions.</param>
    /// <param name="transactions">The transactions to upsert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpsertTransactionsAsync(
        string accountId,
        string userId,
        IEnumerable<Transaction> transactions,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all transactions for a specific account.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of transactions for the account.</returns>
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Calculates and updates running balances for all transactions of an account.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of transactions updated.</returns>
    Task<int> CalculateRunningBalancesAsync(string accountId, CancellationToken cancellationToken = default);
}
