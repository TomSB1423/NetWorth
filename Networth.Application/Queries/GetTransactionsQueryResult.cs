using Networth.Application.Models;
using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     The result of the paginated get transactions query.
/// </summary>
public record GetTransactionsQueryResult
{
    /// <summary>
    ///     Gets the account ID.
    /// </summary>
    public required string AccountId { get; init; }

    /// <summary>
    ///     Gets the paginated transactions.
    /// </summary>
    public required PagedResult<Transaction> Transactions { get; init; }
}
