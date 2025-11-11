using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query for retrieving account balances.
/// </summary>
public class GetAccountBalancesQuery : IRequest<GetAccountBalancesQueryResult>
{
    /// <summary>
    ///     Gets the account ID.
    /// </summary>
    public required string AccountId { get; init; }
}
