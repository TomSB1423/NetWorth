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
        typeof(IEnumerable<TransactionResponse>),
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "accounts/{accountId}/transactions")]
        HttpRequest req,
        string accountId)
    {
        // Read query parameters from the request
        string dateFromStr = req.Query["dateFrom"].ToString();
        string dateToStr = req.Query["dateTo"].ToString();

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

        // Return 404 if account not found
        if (result.Transactions is null)
        {
            logger.LogWarning("Account {AccountId} not found", accountId);
            return new NotFoundResult();
        }

        var response = result.Transactions.Select(t => new TransactionResponse
        {
            Id = t.Id,
            AccountId = t.AccountId,
            TransactionId = t.TransactionId,
            Amount = t.Amount,
            Currency = t.Currency,
            BookingDate = t.BookingDate,
            ValueDate = t.ValueDate,
            RemittanceInformationUnstructured = t.RemittanceInformationUnstructured,
        });

        logger.LogInformation(
            "Successfully retrieved {TransactionCount} transactions for account {AccountId}",
            response.Count(),
            accountId);

        return new OkObjectResult(response);
    }
}
