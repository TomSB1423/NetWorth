using Networth.Domain.Entities;

namespace Networth.Application.Queries;

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
    ///     Gets the transactions for the account from GoCardless.
    /// </summary>
    public IEnumerable<Transaction> Transactions { get; init; } = [];
}
