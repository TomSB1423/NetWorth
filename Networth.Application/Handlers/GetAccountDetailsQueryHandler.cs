using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetAccountDetailsQuery.
/// </summary>
public class GetAccountDetailsQueryHandler(
    IFinancialProvider financialProvider,
    ILogger<GetAccountDetailsQueryHandler> logger)
    : IRequestHandler<GetAccountDetailsQuery, GetAccountDetailsQueryResult>
{
    /// <inheritdoc />
    public async Task<GetAccountDetailsQueryResult> HandleAsync(
        GetAccountDetailsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving details for account {AccountId}", query.AccountId);

        var accountDetails = await financialProvider.GetAccountDetailsAsync(query.AccountId, cancellationToken);

        logger.LogInformation("Successfully retrieved details for account {AccountId}", query.AccountId);

        return new GetAccountDetailsQueryResult
        {
            AccountDetail = accountDetails
        };
    }
}
