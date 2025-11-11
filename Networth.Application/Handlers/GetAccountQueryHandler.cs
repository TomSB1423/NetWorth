using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetAccountQuery.
/// </summary>
public class GetAccountQueryHandler(
    IFinancialProvider financialProvider,
    ILogger<GetAccountQueryHandler> logger)
    : IRequestHandler<GetAccountQuery, GetAccountQueryResult>
{
    /// <inheritdoc />
    public async Task<GetAccountQueryResult> HandleAsync(
        GetAccountQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving account metadata for account {AccountId}", query.AccountId);

        var account = await financialProvider.GetAccountAsync(query.AccountId, cancellationToken);

        logger.LogInformation("Successfully retrieved account metadata for account {AccountId}", query.AccountId);

        return new GetAccountQueryResult
        {
            Account = account
        };
    }
}
