using System.Globalization;
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
///     Azure Function for retrieving account transactions.
/// </summary>
public class GetAccountTransactions(IFinancialProvider financialProvider, ILogger<GetAccountTransactions> logger)
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
        Required = false,
        Type = typeof(string),
        Description = "Start date for transaction filtering (YYYY-MM-DD format)")]
    [OpenApiParameter(
        "dateTo",
        In = ParameterLocation.Query,
        Required = false,
        Type = typeof(string),
        Description = "End date for transaction filtering (YYYY-MM-DD format)")]
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
        string accountId)
    {
        try
        {
            if (string.IsNullOrEmpty(accountId))
            {
                logger.LogWarning("Missing accountId in GetAccountTransactions request");
                return new BadRequestObjectResult("Account ID is required");
            }

            // Parse optional date parameters from query string
            DateTime? dateFrom = null;
            DateTime? dateTo = null;

            if (req.Query.ContainsKey("dateFrom"))
            {
                string dateFromStr = req.Query["dateFrom"].ToString();
                if (!string.IsNullOrEmpty(dateFromStr))
                {
                    if (DateTime.TryParseExact(
                            dateFromStr,
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateTime parsedDateFrom))
                    {
                        dateFrom = parsedDateFrom;
                    }
                    else
                    {
                        logger.LogWarning("Invalid dateFrom format in GetAccountTransactions request: {DateFrom}", dateFromStr);
                        return new BadRequestObjectResult("Invalid dateFrom format. Use YYYY-MM-DD format.");
                    }
                }
            }

            if (req.Query.ContainsKey("dateTo"))
            {
                string dateToStr = req.Query["dateTo"].ToString();
                if (!string.IsNullOrEmpty(dateToStr))
                {
                    if (DateTime.TryParseExact(dateToStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTo))
                    {
                        dateTo = parsedDateTo;
                    }
                    else
                    {
                        logger.LogWarning("Invalid dateTo format in GetAccountTransactions request: {DateTo}", dateToStr);
                        return new BadRequestObjectResult("Invalid dateTo format. Use YYYY-MM-DD format.");
                    }
                }
            }

            var transactions = await financialProvider.GetAccountTransactionsAsync(
                accountId,
                dateFrom,
                dateTo);

            logger.LogInformation(
                "Successfully retrieved {TransactionCount} transactions for account {AccountId}",
                transactions.Count(),
                accountId);

            return new OkObjectResult(transactions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving account transactions for account {AccountId}", accountId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
