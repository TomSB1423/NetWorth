using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Functions.Authentication;

namespace Networth.Functions.Functions.Http.Institutions;

/// <summary>
///     Azure Function for syncing all accounts of an institution.
/// </summary>
public class SyncInstitution(
    IMediator mediator,
    ICurrentUserService currentUserService,
    ILogger<SyncInstitution> logger)
{
    /// <summary>
    ///     Syncs all accounts for a specific institution by enqueueing sync messages for each account.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="institutionId">The institution ID from the route.</param>
    /// <returns>The sync result with number of accounts enqueued.</returns>
    [Function("SyncInstitution")]
    [OpenApiOperation(
        "SyncInstitution",
        "Institutions",
        Summary = "Sync institution accounts",
        Description = "Enqueues sync operations for all accounts belonging to the specified institution for the authenticated user.")]
    [OpenApiParameter(
        "institutionId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The institution ID to sync")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(SyncInstitutionResponse),
        Description = "Successfully enqueued account syncs")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Unauthorized,
        Description = "User is not authenticated")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid request parameters")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "institutions/{institutionId}/sync")]
        HttpRequest req,
        string institutionId)
    {
        if (!currentUserService.IsAuthenticated)
        {
            logger.LogWarning("Unauthenticated request to SyncInstitution");
            return new UnauthorizedResult();
        }

        logger.LogInformation(
            "Received request to sync institution {InstitutionId} for user {UserId}",
            institutionId,
            currentUserService.UserId);

        var command = new SyncInstitutionCommand
        {
            InstitutionId = institutionId,
            UserId = currentUserService.UserId,
        };

        var result = await mediator.Send<SyncInstitutionCommand, SyncInstitutionCommandResult>(command);

        logger.LogInformation(
            "Successfully enqueued {Count} accounts for sync from institution {InstitutionId}",
            result.AccountsEnqueued,
            institutionId);

        var response = new SyncInstitutionResponse
        {
            InstitutionId = result.InstitutionId,
            AccountsEnqueued = result.AccountsEnqueued,
            AccountIds = result.AccountIds.ToList(),
        };

        return new OkObjectResult(response);
    }

    /// <summary>
    ///     Response for institution sync operation.
    /// </summary>
    private class SyncInstitutionResponse
    {
        /// <summary>
        ///     Gets or sets the institution ID that was synced.
        /// </summary>
        public required string InstitutionId { get; set; }

        /// <summary>
        ///     Gets or sets the number of accounts enqueued for sync.
        /// </summary>
        public required int AccountsEnqueued { get; set; }

        /// <summary>
        ///     Gets or sets the list of account IDs that were enqueued.
        /// </summary>
        public required List<string> AccountIds { get; set; }
    }
}
