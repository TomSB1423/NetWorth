using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetAccountsQuery.
/// </summary>
public class GetAccountsQueryResult
{
    /// <summary>
    ///     Gets the list of accounts.
    /// </summary>
    public required IEnumerable<AccountDto> Accounts { get; init; }
}
