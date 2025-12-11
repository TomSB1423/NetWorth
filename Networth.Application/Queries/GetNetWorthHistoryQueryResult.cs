using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result of the GetNetWorthHistoryQuery.
/// </summary>
/// <param name="DataPoints">The collection of net worth data points.</param>
public record GetNetWorthHistoryQueryResult(IEnumerable<NetWorthPoint> DataPoints);
