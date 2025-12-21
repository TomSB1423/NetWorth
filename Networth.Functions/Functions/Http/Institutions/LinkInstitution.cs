using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Institutions;

/// <summary>
///     Azure Function for linking institutions by creating agreements and requisitions.
/// </summary>
public class LinkInstitution(IMediator mediator, ICurrentUserService currentUserService)
{
    /// <summary>
    ///     Links an institution by creating an agreement and requisition in sequence.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="institutionId">The institution ID from the route.</param>
    /// <returns>The created requisition details with authorization link.</returns>
    [Function("LinkInstitution")]
    [OpenApiOperation(
        "LinkInstitution",
        "Institutions",
        Summary = "Link institution",
        Description = "Links an institution by creating an agreement with the financial institution and then creating a requisition for account access.")]
    [OpenApiParameter(
        "institutionId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The institution ID to link")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(LinkInstitutionResponse),
        Description = "Successfully linked institution")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid request parameters")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "institutions/{institutionId}/link")]
        HttpRequest req,
        string institutionId)
    {
        var command = new LinkInstitutionCommand
        {
            UserId = currentUserService.InternalUserId!.Value,
            InstitutionId = institutionId,
        };
        var result = await mediator.Send<LinkInstitutionCommand, LinkInstitutionCommandResult>(command);

        var response = new LinkInstitutionResponse
        {
            AuthorizationLink = result.AuthorizationLink,
            Status = result.Status,
            IsAlreadyLinked = result.IsAlreadyLinked,
            ExistingRequisitionId = result.ExistingRequisitionId,
        };

        return new OkObjectResult(response);
    }
}
