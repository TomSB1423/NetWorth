using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for retrieving user accounts.
/// </summary>
public class GetAccountsQueryHandler(
    IAccountRepository accountRepository,
    ILogger<GetAccountsQueryHandler> logger)
    : IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>
{
    /// <inheritdoc />
    public async Task<GetAccountsQueryResult> HandleAsync(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAccountsQuery for user {UserId}", request.UserId);

        var accounts = await accountRepository.GetAccountsByUserIdAsync(request.UserId, cancellationToken);

        logger.LogInformation("Successfully retrieved {Count} accounts for user {UserId}", accounts.Count(), request.UserId);

        return new GetAccountsQueryResult
        {
            Accounts = accounts,
        };
    }
}
