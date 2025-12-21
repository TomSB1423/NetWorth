using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Accounts;

/// <summary>
///     Azure Function for retrieving all accounts for the current user.
/// </summary>
public class GetAccounts(
    IMediator mediator,
    ICurrentUserService currentUserService,
    ILogger<GetAccounts> logger)
{
    /// <summary>
    ///     Gets all accounts for the current authenticated user.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>A list of accounts owned by the current user.</returns>
    [Function("GetAccounts")]
    [OpenApiOperation(
        "GetAccounts",
        "Accounts",
        Summary = "Get user accounts",
        Description = "Retrieves all bank accounts owned by the current authenticated user.")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(IEnumerable<UserAccountResponse>),
        Description = "Successfully retrieved user accounts")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Unauthorized,
        Description = "User is not authenticated")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "accounts")]
        HttpRequest req)
    {
        if (!currentUserService.IsAuthenticated)
        {
            logger.LogWarning("Unauthenticated request to GetAccounts");
            return new UnauthorizedResult();
        }

        var query = new GetAccountsQuery
        {
            UserId = currentUserService.InternalUserId!.Value,
        };

        var result = await mediator.Send<GetAccountsQuery, GetAccountsQueryResult>(query);

        var response = result.Accounts.Select(a => new UserAccountResponse
        {
            Id = a.Id,
            UserId = a.UserId,
            RequisitionId = a.RequisitionId,
            InstitutionId = a.InstitutionId,
            InstitutionName = a.InstitutionName,
            InstitutionLogo = a.InstitutionLogo,
            Name = a.Name,
            DisplayName = a.DisplayName,
            Category = a.Category,
            Iban = a.Iban,
            Currency = a.Currency,
            Product = a.Product,
            LastSynced = a.LastSynced,
        });

        return new OkObjectResult(response);
    }
}
