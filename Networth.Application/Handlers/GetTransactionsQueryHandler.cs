using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Entities;

namespace Networth.Application.Handlers;

public class GetTransactionsQueryHandler(IFinancialProvider financialProvider, ILogger<GetTransactionsQueryHandler> logger)
    : IRequestHandler<GetTransactionsQuery, GetTransactionsQueryResult>
{
    public async Task<GetTransactionsQueryResult> HandleAsync(GetTransactionsQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting transactions for account: {QueryAccountId}", query.AccountId);
        IEnumerable<Transaction>? response = await financialProvider.GetAccountTransactionsAsync(
            query.AccountId,
            query.DateFrom,
            query.DateTo,
            cancellationToken);

        return new GetTransactionsQueryResult
        {
            AccountId = query.AccountId, Transactions = response,
        };
    }
}
