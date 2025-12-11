using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query for retrieving the net worth history for a user.
/// </summary>
/// <param name="UserId">The user ID.</param>
public record GetNetWorthHistoryQuery(string UserId) : IRequest<GetNetWorthHistoryQueryResult>;
