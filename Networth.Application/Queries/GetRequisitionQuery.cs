using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query for retrieving a requisition.
/// </summary>
public class GetRequisitionQuery : IRequest<GetRequisitionQueryResult>
{
    /// <summary>
    ///     Gets the requisition ID.
    /// </summary>
    public required string RequisitionId { get; init; }
}
