using Networth.Domain.Entities;
using Networth.Domain.Enums;

namespace Networth.Application.Queries;

/// <summary>
///     Result of the GetNetWorthHistoryQuery.
/// </summary>
/// <param name="DataPoints">The collection of net worth data points.</param>
/// <param name="Status">The calculation status.</param>
/// <param name="LastCalculated">The date and time when the net worth was last calculated.</param>
public record GetNetWorthHistoryQueryResult(
    IEnumerable<NetWorthPoint> DataPoints,
    NetWorthCalculationStatus Status,
    DateTime? LastCalculated);
