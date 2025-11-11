using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetAccountBalancesQuery.
/// </summary>
public class GetAccountBalancesQueryResult
{
    /// <summary>
    ///     Gets the account balances.
    /// </summary>
    public required IEnumerable<AccountBalance> Balances { get; init; }
}
