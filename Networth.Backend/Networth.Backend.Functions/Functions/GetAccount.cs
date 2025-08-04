using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Functions.Functions;

/// <summary>
///     Azure Function for retrieving account metadata.
/// </summary>
public class GetAccount(IFinancialProvider financialProvider, ILogger<GetAccount> logger)
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
        In = Microsoft.OpenApi.Models.ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The account ID")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(Account),
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
        try
        {
            if (string.IsNullOrEmpty(accountId))
            {
                logger.LogWarning("Missing accountId in GetAccount request");
                return new BadRequestObjectResult("Account ID is required");
            }

            Account account = await financialProvider.GetAccountAsync(accountId);

            logger.LogInformation("Successfully retrieved account metadata for account {AccountId}", accountId);

            return new OkObjectResult(account);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving account {AccountId}", accountId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
