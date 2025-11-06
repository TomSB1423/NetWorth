using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Application.Queries;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Application.Handlers;

public class GetTransactionsQueryHandler(IFinancialProvider financialProvider, ILogger<GetTransactionsQueryHandler> logger)
    : IRequestHandler<GetTransactionsQuery, GetTransactionsQueryResult>
{
    public async Task<GetTransactionsQueryResult> HandleAsync(GetTransactionsQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting transactions for account: {QueryAccountId}", query.AccountId);
        IEnumerable<TransactionMetadata> response = await financialProvider.GetAccountTransactionsAsync(
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
