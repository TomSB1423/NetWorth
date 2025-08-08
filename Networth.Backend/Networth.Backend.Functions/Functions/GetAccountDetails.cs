using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Functions.Functions;

/// <summary>
///     Azure Function for retrieving detailed account information.
/// </summary>
public class GetAccountDetails(IFinancialProvider financialProvider, ILogger<GetAccountDetails> logger)
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
        typeof(AccountDetail),
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
        try
        {
            if (string.IsNullOrEmpty(accountId))
            {
                logger.LogWarning("Missing accountId in GetAccountDetails request");
                return new BadRequestObjectResult("Account ID is required");
            }

            AccountDetail accountDetails = await financialProvider.GetAccountDetailsAsync(accountId);

            logger.LogInformation("Successfully retrieved account details for account {AccountId}", accountId);

            return new OkObjectResult(accountDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving account details for account {AccountId}", accountId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
