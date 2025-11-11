using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetAccountDetailsQuery.
/// </summary>
public class GetAccountDetailsQueryResult
{
    /// <summary>
    ///     Gets the account details.
    /// </summary>
    public required AccountDetail AccountDetail { get; init; }
}
