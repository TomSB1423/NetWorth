using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetAccountDetailsQuery.
/// </summary>
public class GetAccountDetailsQueryResult
{
    /// <summary>
    ///     Gets the account details, or null if the account was not found.
    /// </summary>
    public required AccountDetail? AccountDetail { get; init; }
}
