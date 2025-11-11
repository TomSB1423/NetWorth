using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetRequisitionQuery.
/// </summary>
public class GetRequisitionQueryHandler(
    IFinancialProvider financialProvider,
    ILogger<GetRequisitionQueryHandler> logger)
    : IRequestHandler<GetRequisitionQuery, GetRequisitionQueryResult>
{
    /// <inheritdoc />
    public async Task<GetRequisitionQueryResult> HandleAsync(
        GetRequisitionQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(query.RequisitionId))
        {
            logger.LogWarning("Missing RequisitionId in GetRequisitionQuery");
            throw new ArgumentException("Requisition ID is required", nameof(query.RequisitionId));
        }

        logger.LogInformation("Retrieving requisition {RequisitionId}", query.RequisitionId);

        var requisition = await financialProvider.GetRequisitionAsync(query.RequisitionId, cancellationToken);

        logger.LogInformation("Successfully retrieved requisition {RequisitionId}", query.RequisitionId);

        return new GetRequisitionQueryResult
        {
            Requisition = requisition
        };
    }
}
