using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query to get paginated transactions for an account.
/// </summary>
public class GetTransactionsQuery : IRequest<GetTransactionsQueryResult>
{
    /// <summary>
    ///     Gets the account ID to retrieve transactions from.
    /// </summary>
    public required string AccountId { get; init; }

    /// <summary>
    ///     Gets the page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    ///     Gets the number of items per page.
    /// </summary>
    public int PageSize { get; init; } = 50;
}
