using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetNetWorthHistoryQuery.
/// </summary>
public class GetNetWorthHistoryQueryHandler(ITransactionRepository transactionRepository)
    : IRequestHandler<GetNetWorthHistoryQuery, GetNetWorthHistoryQueryResult>
{
    /// <inheritdoc />
    public async Task<GetNetWorthHistoryQueryResult> HandleAsync(
        GetNetWorthHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var history = await transactionRepository.GetNetWorthHistoryAsync(request.UserId, cancellationToken);
        return new GetNetWorthHistoryQueryResult(history);
    }
}
