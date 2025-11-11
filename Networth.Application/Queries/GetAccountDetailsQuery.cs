using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query for retrieving account details.
/// </summary>
public class GetAccountDetailsQuery : IRequest<GetAccountDetailsQueryResult>
{
    /// <summary>
    ///     Gets the account ID.
    /// </summary>
    public required string AccountId { get; init; }
}
