using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Networth.Domain.Entities;
using Networth.Functions.Authentication;
using Networth.Infrastructure.Data.Context;

namespace Networth.Functions.Functions;

/// <summary>
///     Azure Function for retrieving all accounts for the current user.
/// </summary>
public class GetAccounts(
    NetworthDbContext dbContext,
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
        typeof(IEnumerable<Account>),
        Description = "Successfully retrieved user accounts")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Unauthorized,
        Description = "User is not authenticated")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")]
        HttpRequest req)
    {
        if (!currentUserService.IsAuthenticated)
        {
            logger.LogWarning("Unauthenticated request to GetAccounts");
            return new UnauthorizedResult();
        }

        string userId = currentUserService.UserId;
        logger.LogInformation("Retrieving accounts for user {UserId}", userId);

        List<Account> accounts = await dbContext.Accounts
            .Where(a => a.OwnerId == userId)
            .Include(a => a.Institution)
            .OrderBy(a => a.Name)
            .ToListAsync();

        logger.LogInformation(
            "Successfully retrieved {Count} accounts for user {UserId}",
            accounts.Count,
            userId);

        return new OkObjectResult(accounts);
    }
}
