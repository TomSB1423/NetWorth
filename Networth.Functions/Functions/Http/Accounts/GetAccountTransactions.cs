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
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 100;

    /// <summary>
    ///     Gets paginated transactions for a specific account.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="accountId">The account ID from the route.</param>
    /// <returns>The paginated account transactions.</returns>
    [Function("GetAccountTransactions")]
    [OpenApiOperation(
        "GetAccountTransactions",
        "Accounts",
        Summary = "Get account transactions",
        Description = "Retrieves paginated transactions for a specific bank account.")]
    [OpenApiParameter(
        "accountId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The account ID")]
    [OpenApiParameter(
        "page",
        In = ParameterLocation.Query,
        Required = false,
        Type = typeof(int),
        Description = "Page number (1-based, default: 1)")]
    [OpenApiParameter(
        "pageSize",
        In = ParameterLocation.Query,
        Required = false,
        Type = typeof(int),
        Description = "Number of items per page (default: 50, max: 100)")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(PagedResponse<TransactionResponse>),
        Description = "Successfully retrieved paginated account transactions")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid parameters")]
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
        // Parse pagination parameters
        int page = 1;
        int pageSize = DefaultPageSize;

        if (req.Query.TryGetValue("page", out var pageStr) && int.TryParse(pageStr, out var parsedPage))
        {
            page = Math.Max(1, parsedPage);
        }

        if (req.Query.TryGetValue("pageSize", out var pageSizeStr) && int.TryParse(pageSizeStr, out var parsedPageSize))
        {
            pageSize = Math.Clamp(parsedPageSize, 1, MaxPageSize);
        }

        logger.LogInformation(
            "Received request to get paginated transactions for account {AccountId}, page {Page}, pageSize {PageSize}",
            accountId,
            page,
            pageSize);

        // Create query object
        GetTransactionsQuery query = new()
        {
            AccountId = accountId,
            Page = page,
            PageSize = pageSize,
        };

        // Send through mediator (includes validation)
        GetTransactionsQueryResult result = await mediator.Send<GetTransactionsQuery, GetTransactionsQueryResult>(query);

        var transactionItems = result.Transactions.Items.Select(t => new TransactionResponse
        {
            Id = t.Id,
            AccountId = t.AccountId,
            TransactionId = t.TransactionId,
            Amount = t.Amount,
            Currency = t.Currency,
            BookingDate = t.BookingDate,
            ValueDate = t.ValueDate,
            RemittanceInformationUnstructured = t.RemittanceInformationUnstructured,
            DebtorName = t.DebtorName,
            DebtorAccount = t.DebtorAccount,
        });

        var response = new PagedResponse<TransactionResponse>
        {
            Items = transactionItems,
            Page = result.Transactions.Page,
            PageSize = result.Transactions.PageSize,
            TotalCount = result.Transactions.TotalCount,
            TotalPages = result.Transactions.TotalPages,
            HasNextPage = result.Transactions.HasNextPage,
            HasPreviousPage = result.Transactions.HasPreviousPage,
        };

        logger.LogInformation(
            "Successfully retrieved page {Page} of {TotalPages} ({Count} items) for account {AccountId}",
            response.Page,
            response.TotalPages,
            response.Items.Count(),
            accountId);

        return new OkObjectResult(response);
    }
}
