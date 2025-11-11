using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetAccountQuery.
/// </summary>
public class GetAccountQueryResult
{
    /// <summary>
    ///     Gets the account metadata.
    /// </summary>
    public required Account Account { get; init; }
}
