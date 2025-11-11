using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetRequisitionQuery.
/// </summary>
public class GetRequisitionQueryResult
{
    /// <summary>
    ///     Gets the requisition.
    /// </summary>
    public required Requisition Requisition { get; init; }
}
