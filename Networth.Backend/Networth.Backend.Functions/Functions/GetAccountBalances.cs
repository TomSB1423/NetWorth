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
///     Azure Function for retrieving account balances.
/// </summary>
public class GetAccountBalances(IFinancialProvider financialProvider, ILogger<GetAccountBalances> logger)
{
    /// <summary>
    ///     Gets the balances for a specific account.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="accountId">The account ID from the route.</param>
    /// <returns>The account balances.</returns>
    [Function("GetAccountBalances")]
    [OpenApiOperation(
        "GetAccountBalances",
        "Accounts",
        Summary = "Get account balances",
        Description = "Retrieves the current balances for a specific bank account.")]
    [OpenApiParameter(
        "accountId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The account ID")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(IEnumerable<AccountBalance>),
        Description = "Successfully retrieved account balances")]
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
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}/balances")]
        HttpRequest req,
        string accountId)
    {
        IEnumerable<AccountBalance> balances = await financialProvider.GetAccountBalancesAsync(accountId);
        logger.LogInformation("Successfully retrieved account balances for account {AccountId}", accountId);
        return new OkObjectResult(balances);
    }
}
