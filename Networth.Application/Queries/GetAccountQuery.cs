using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query for retrieving account metadata.
/// </summary>
public class GetAccountQuery : IRequest<GetAccountQueryResult>
{
    /// <summary>
    ///     Gets the account ID.
    /// </summary>
    public required string AccountId { get; init; }
}
