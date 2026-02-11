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

namespace Networth.Functions.Functions.Http.Accounts;

/// <summary>
///     Azure Function for retrieving account balances.
/// </summary>
public class GetAccountBalances(IMediator mediator, ILogger<GetAccountBalances> logger)
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
        typeof(IEnumerable<AccountBalanceResponse>),
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "accounts/{accountId}/balances")]
        HttpRequest req,
        string accountId)
    {
        logger.LogInformation("Received request to get balances for account {AccountId}", accountId);

        var query = new GetAccountBalancesQuery { AccountId = accountId };
        var result = await mediator.Send<GetAccountBalancesQuery, GetAccountBalancesQueryResult>(query);

        // Return 404 if account not found
        if (result.Balances is null)
        {
            logger.LogWarning("Account {AccountId} not found", accountId);
            return new NotFoundResult();
        }

        var response = result.Balances.Select(b => new AccountBalanceResponse
        {
            Amount = b.Amount,
            Currency = b.Currency,
            BalanceType = b.BalanceType,
            CreditLimitIncluded = b.CreditLimitIncluded,
            ReferenceDate = b.ReferenceDate,
        });

        logger.LogInformation("Successfully retrieved account balances for account {AccountId}", accountId);
        return new OkObjectResult(response);
    }
}
