using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for retrieving user accounts.
/// </summary>
public class GetAccountsQueryResult
{
    /// <summary>
    ///     Gets the collection of accounts.
    /// </summary>
    public required IEnumerable<UserAccount> Accounts { get; init; }
}
