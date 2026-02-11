using Networth.Domain.Enums;

namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for net worth history endpoint.
/// </summary>
public class NetWorthHistoryResponse
{
    /// <summary>
    ///     Gets the collection of net worth data points.
    /// </summary>
    public required IEnumerable<NetWorthPointResponse> DataPoints { get; init; }

    /// <summary>
    ///     Gets the calculation status.
    /// </summary>
    public required NetWorthCalculationStatus Status { get; init; }

    /// <summary>
    ///     Gets the date and time when the net worth was last calculated.
    /// </summary>
    public DateTime? LastCalculated { get; init; }
}
