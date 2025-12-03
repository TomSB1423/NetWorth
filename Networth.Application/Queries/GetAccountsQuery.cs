using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query for retrieving all accounts for a user.
/// </summary>
public class GetAccountsQuery : IRequest<GetAccountsQueryResult>
{
    /// <summary>
    ///     Gets the user ID to retrieve accounts for.
    /// </summary>
    public required string UserId { get; init; }
}
