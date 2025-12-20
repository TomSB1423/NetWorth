using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Statistics;

/// <summary>
///     Azure Function for retrieving the net worth history for the current user.
/// </summary>
public class GetNetWorthHistory(
    IMediator mediator,
    ICurrentUserService currentUserService,
    ILogger<GetNetWorthHistory> logger)
{
    /// <summary>
    ///     Gets the net worth history for the current authenticated user.
    /// </summary>
    /// <returns>A collection of net worth data points with status and last calculated timestamp.</returns>
    [Function("GetNetWorthHistory")]
    [OpenApiOperation(
        "GetNetWorthHistory",
        "Statistics",
        Summary = "Get net worth history",
        Description = "Retrieves the daily net worth history for the current authenticated user.")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(NetWorthHistoryResponse),
        Description = "Successfully retrieved net worth history")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Unauthorized,
        Description = "User is not authenticated")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "statistics/net-worth")] HttpRequest unused)
    {
        var userId = currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Unauthorized access attempt to GetNetWorthHistory");
            return new UnauthorizedResult();
        }

        logger.LogInformation("Retrieving net worth history for user {UserId}", userId);

        var query = new GetNetWorthHistoryQuery(userId);
        var result = await mediator.Send<GetNetWorthHistoryQuery, GetNetWorthHistoryQueryResult>(query);

        var response = new NetWorthHistoryResponse
        {
            DataPoints = result.DataPoints.Select(p => new NetWorthPointResponse
            {
                Date = p.Date,
                Amount = p.Amount
            }),
            Status = result.Status,
            LastCalculated = result.LastCalculated
        };

        return new OkObjectResult(response);
    }
}
