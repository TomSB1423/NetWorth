using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetAccountQuery.
/// </summary>
public class GetAccountQueryResult
{
    /// <summary>
    ///     Gets the account metadata, or null if not found.
    /// </summary>
    public required Account? Account { get; init; }
}
