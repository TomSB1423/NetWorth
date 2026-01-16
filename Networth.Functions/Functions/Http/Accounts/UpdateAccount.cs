using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Entities;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Requests;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Accounts;

/// <summary>
///     Azure Function to update an account's user-defined fields.
/// </summary>
public class UpdateAccount(
    IMediator mediator,
    ICurrentUserService currentUserService,
    ILogger<UpdateAccount> logger)
{
    /// <summary>
    ///     Updates an account's display name and type.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="accountId">The account ID to update.</param>
    /// <returns>The updated account details.</returns>
    [Function("UpdateAccount")]
    [OpenApiOperation(
        "UpdateAccount",
        "Accounts",
        Summary = "Update account fields",
        Description = "Updates an account's user-defined fields such as display name and account type.")]
    [OpenApiParameter(
        "accountId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The account ID")]
    [OpenApiRequestBody(
        "application/json",
        typeof(UpdateAccountRequest),
        Description = "The fields to update")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(UserAccountResponse),
        Description = "Account updated successfully")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Unauthorized,
        Description = "User is not authenticated")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.NotFound,
        Description = "Account not found")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid request")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "accounts/{accountId}")]
        HttpRequest req,
        string accountId)
    {
        if (!currentUserService.IsAuthenticated)
        {
            logger.LogWarning("Unauthenticated request to UpdateAccount");
            return new UnauthorizedResult();
        }

        UpdateAccountRequest? request;
        try
        {
            request = await req.ReadFromJsonAsync<UpdateAccountRequest>();
        }
        catch
        {
            return new BadRequestObjectResult("Invalid request body");
        }

        if (request == null)
        {
            return new BadRequestObjectResult("Request body is required");
        }

        var command = new UpdateAccountCommand
        {
            AccountId = accountId,
            UserId = currentUserService.InternalUserId!.Value,
            DisplayName = request.DisplayName,
            Category = request.Category,
        };

        try
        {
            var result = await mediator.Send<UpdateAccountCommand, UserAccount>(command);

            return new OkObjectResult(new UserAccountResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                RequisitionId = result.RequisitionId,
                InstitutionId = result.InstitutionId,
                InstitutionName = result.InstitutionName,
                InstitutionLogo = result.InstitutionLogo,
                Name = result.Name,
                DisplayName = result.DisplayName,
                Category = result.Category,
                Currency = result.Currency,
                Product = result.Product,
            });
        }
        catch (InvalidOperationException)
        {
            logger.LogWarning("Account {AccountId} not found for user", accountId);
            return new NotFoundResult();
        }
    }
}
