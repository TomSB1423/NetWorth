using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     The get transactions query.
/// </summary>
public class GetTransactionsQuery : IRequest<GetTransactionsQueryResult>
{
    /// <summary>
    ///     Gets the account to retrieve transactions from.
    /// </summary>
    public required string AccountId { get; init; }

    /// <summary>
    ///     Gets the start date of the transactions.
    /// </summary>
    public required DateTimeOffset DateFrom { get; init; }

    /// <summary>
    ///     Gets the end date of the transactions.
    /// </summary>
    public required DateTimeOffset DateTo { get; init; }
}
