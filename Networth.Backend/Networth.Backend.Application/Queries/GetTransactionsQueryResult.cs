using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Application.Queries;

/// <summary>
///     The result of the get transactions query.
/// </summary>
public record GetTransactionsQueryResult
{
    /// <summary>
    ///     Gets the account id.
    /// </summary>
    public required string AccountId { get; init; }

    /// <summary>
    ///     Gets the transactions for the account.
    /// </summary>
    public IEnumerable<Transaction> Transactions { get; init; } = [];
}
