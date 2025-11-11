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
///     Azure Function for retrieving detailed account information.
/// </summary>
public class GetAccountDetails(IMediator mediator, ILogger<GetAccountDetails> logger)
{
    /// <summary>
    ///     Gets the detailed information for a specific account.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="accountId">The account ID from the route.</param>
    /// <returns>The detailed account information.</returns>
    [Function("GetAccountDetails")]
    [OpenApiOperation(
        "GetAccountDetails",
        "Accounts",
        Summary = "Get account details",
        Description = "Retrieves detailed information for a specific bank account including owner details and account properties.")]
    [OpenApiParameter(
        "accountId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The account ID")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(AccountDetailResponse),
        Description = "Successfully retrieved account details")]
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
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}/details")]
        HttpRequest req,
        string accountId)
    {
        logger.LogInformation("Received request to get details for account {AccountId}", accountId);

        var query = new GetAccountDetailsQuery { AccountId = accountId };
        var result = await mediator.Send<GetAccountDetailsQuery, GetAccountDetailsQueryResult>(query);

        var response = new AccountDetailResponse
        {
            Id = result.AccountDetail.Id,
            Currency = result.AccountDetail.Currency,
            Name = result.AccountDetail.Name,
            DisplayName = result.AccountDetail.DisplayName,
            Product = result.AccountDetail.Product,
            CashAccountType = result.AccountDetail.CashAccountType,
            Status = result.AccountDetail.Status,
        };

        logger.LogInformation("Successfully retrieved account details for account {AccountId}", accountId);
        return new OkObjectResult(response);
    }
}
