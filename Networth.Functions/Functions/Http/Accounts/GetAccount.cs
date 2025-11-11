using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions;

/// <summary>
///     Azure Function for retrieving account metadata.
/// </summary>
public class GetAccount(IMediator mediator, ILogger<GetAccount> logger)
{
    /// <summary>
    ///     Gets the metadata for a specific account.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="accountId">The account ID from the route.</param>
    /// <returns>The account metadata.</returns>
    [Function("GetAccount")]
    [OpenApiOperation(
        "GetAccount",
        "Accounts",
        Summary = "Get account metadata",
        Description = "Retrieves the metadata for a specific bank account.")]
    [OpenApiParameter(
        "accountId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The account ID")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(AccountResponse),
        Description = "Successfully retrieved account metadata")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid account ID")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.NotFound,
        Description = "Account not found")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}")]
        HttpRequest req,
        string accountId)
    {
        logger.LogInformation("Received request to get account metadata for {AccountId}", accountId);

        var query = new GetAccountQuery { AccountId = accountId };
        var result = await mediator.Send<GetAccountQuery, GetAccountQueryResult>(query);

        var response = new AccountResponse
        {
            Id = result.Account.Id,
            InstitutionId = result.Account.InstitutionId,
            Status = result.Account.Status,
            Name = result.Account.Name,
        };

        logger.LogInformation("Successfully retrieved account metadata for account {AccountId}", accountId);
        return new OkObjectResult(response);
    }
}
