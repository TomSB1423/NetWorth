using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Enums;
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
        var dataPoints = history.ToList();

        // Determine status and last calculated date
        NetWorthCalculationStatus status;
        DateTime? lastCalculated;

        if (dataPoints.Count == 0)
        {
            status = NetWorthCalculationStatus.NotCalculated;
            lastCalculated = null;
        }
        else
        {
            status = NetWorthCalculationStatus.Calculated;
            lastCalculated = dataPoints.Max(p => p.Date);
        }

        return new GetNetWorthHistoryQueryResult(dataPoints, status, lastCalculated);
    }
}
