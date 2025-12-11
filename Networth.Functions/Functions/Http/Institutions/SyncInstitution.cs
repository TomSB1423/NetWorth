using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Networth.Application.Interfaces;
using Networth.Functions.Authentication;

namespace Networth.Functions.Functions.Http.Institutions;

/// <summary>
///     Azure Function for syncing all accounts of an institution.
/// </summary>
[AllowAnonymous]
public class SyncInstitution(
    IQueueService queueService,
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
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Accepted,
        Description = "Successfully enqueued institution sync")]
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "institutions/{institutionId}/sync")]
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

        await queueService.EnqueueInstitutionSyncAsync(institutionId, currentUserService.UserId);

        logger.LogInformation(
            "Successfully enqueued institution sync for institution {InstitutionId}",
            institutionId);

        return new AcceptedResult();
    }
}
