using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetAccountsQuery.
/// </summary>
public class GetAccountsQueryHandler(
    IAccountService accountService,
    ILogger<GetAccountsQueryHandler> logger)
    : IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>
{
    /// <inheritdoc />
    public async Task<GetAccountsQueryResult> HandleAsync(
        GetAccountsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAccountsQuery for user {UserId}", query.UserId);

        var accounts = await accountService.GetAccountsByUserIdAsync(query.UserId, cancellationToken);

        return new GetAccountsQueryResult
        {
            Accounts = accounts
        };
    }
}
