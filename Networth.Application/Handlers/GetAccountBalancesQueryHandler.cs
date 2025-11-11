using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetAccountBalancesQuery.
/// </summary>
public class GetAccountBalancesQueryHandler(
    IFinancialProvider financialProvider,
    ILogger<GetAccountBalancesQueryHandler> logger)
    : IRequestHandler<GetAccountBalancesQuery, GetAccountBalancesQueryResult>
{
    /// <inheritdoc />
    public async Task<GetAccountBalancesQueryResult> HandleAsync(
        GetAccountBalancesQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving balances for account {AccountId}", query.AccountId);

        var balances = await financialProvider.GetAccountBalancesAsync(query.AccountId, cancellationToken);

        logger.LogInformation("Successfully retrieved balances for account {AccountId}", query.AccountId);

        return new GetAccountBalancesQueryResult
        {
            Balances = balances
        };
    }
}
