using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Entities;

namespace Networth.Functions.Functions;

/// <summary>
///     Azure Function for retrieving account transactions.
/// </summary>
public class GetAccountTransactions(
    IMediator mediator,
    ILogger<GetAccountTransactions> logger)
{
    /// <summary>
    ///     Gets the transactions for a specific account with optional date filtering.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="accountId">The account ID from the route.</param>
    /// <param name="dateFromStr">The date to get transactions from.</param>
    /// <param name="dateToStr">The date to get transactions till.</param>
    /// <returns>The account transactions.</returns>
    [Function("GetAccountTransactions")]
    [OpenApiOperation(
        "GetAccountTransactions",
        "Accounts",
        Summary = "Get account transactions",
        Description = "Retrieves transactions for a specific bank account with optional date range filtering.")]
    [OpenApiParameter(
        "accountId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The account ID")]
    [OpenApiParameter(
        "dateFrom",
        In = ParameterLocation.Query,
        Required = true,
        Type = typeof(string),
        Description = "Start date for transaction filtering")]
    [OpenApiParameter(
        "dateTo",
        In = ParameterLocation.Query,
        Required = true,
        Type = typeof(string),
        Description = "End date for transaction filtering")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(IEnumerable<Transaction>),
        Description = "Successfully retrieved account transactions")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid account ID or date format")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.NotFound,
        Description = "Account not found")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}/transactions")]
        HttpRequest req,
        string accountId,
        [FromQuery(Name = "dateFrom")] string dateFromStr,
        [FromQuery(Name = "DateTo")] string dateToStr)
    {
        logger.LogInformation(
            "Received request to get transactions for account {AccountId} from {DateFrom} to {DateTo}",
            accountId,
            dateFromStr,
            dateToStr);

        // Parse date strings to DateTimeOffset for validation
        if (!DateTimeOffset.TryParse(dateFromStr, out DateTimeOffset dateFrom))
        {
            logger.LogWarning("Invalid dateFrom format: {DateFromStr}", dateFromStr);
            return new BadRequestObjectResult(new { errors = new[] { $"Invalid dateFrom format: {dateFromStr}. Expected ISO 8601 format." } });
        }

        if (!DateTimeOffset.TryParse(dateToStr, out DateTimeOffset dateTo))
        {
            logger.LogWarning("Invalid dateTo format: {DateToStr}", dateToStr);
            return new BadRequestObjectResult(new { errors = new[] { $"Invalid dateTo format: {dateToStr}. Expected ISO 8601 format." } });
        }

        // Create query object
        GetTransactionsQuery query = new()
        {
            AccountId = accountId,
            DateFrom = dateFrom,
            DateTo = dateTo,
        };

        // Send through mediator (includes validation)
        GetTransactionsQueryResult result = await mediator.Send<GetTransactionsQuery, GetTransactionsQueryResult>(query);

        logger.LogInformation(
            "Successfully retrieved {TransactionCount} transactions for account {AccountId}",
            result.Transactions.Count(),
            accountId);

        return new OkObjectResult(result.Transactions);
    }
}
