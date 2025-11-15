using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetRequisitionQuery.
/// </summary>
public class GetRequisitionQueryHandler(
    IFinancialProvider financialProvider,
    IRequisitionRepository requisitionRepository,
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

        // Try to get from database first
        var cachedRequisition = await requisitionRepository.GetRequisitionByIdAsync(query.RequisitionId, cancellationToken);

        if (cachedRequisition != null)
        {
            logger.LogInformation(
                "Retrieved requisition {RequisitionId} from cache",
                query.RequisitionId);

            return new GetRequisitionQueryResult
            {
                Requisition = cachedRequisition,
            };
        }

        // If not in cache, fetch from GoCardless
        logger.LogInformation(
            "Requisition {RequisitionId} not in cache, fetching from GoCardless",
            query.RequisitionId);

        var requisition = await financialProvider.GetRequisitionAsync(query.RequisitionId, cancellationToken);

        if (requisition == null)
        {
            logger.LogWarning("Requisition {RequisitionId} not found", query.RequisitionId);
            throw new InvalidOperationException($"Requisition {query.RequisitionId} not found");
        }

        logger.LogInformation("Successfully retrieved requisition {RequisitionId}", query.RequisitionId);

        return new GetRequisitionQueryResult
        {
            Requisition = requisition,
        };
    }
}
