using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Requisitions;

/// <summary>
///     Azure Function for retrieving requisition details.
/// </summary>
[Authorize]
public class GetRequisition(IMediator mediator, ILogger<GetRequisition> logger)
{
    /// <summary>
    ///     Gets the details of a specific requisition.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="requisitionId">The requisition ID from the route.</param>
    /// <returns>The requisition details.</returns>
    [Function("GetRequisition")]
    [OpenApiOperation(
        "GetRequisition",
        "Requisitions",
        Summary = "Get requisition",
        Description = "Retrieves the details of a specific requisition by ID.")]
    [OpenApiParameter(
        "requisitionId",
        In = ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The requisition ID")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(RequisitionResponse),
        Description = "Successfully retrieved requisition")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid requisition ID")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.NotFound,
        Description = "Requisition not found")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "requisitions/{requisitionId}")]
        HttpRequest req,
        string requisitionId)
    {
        logger.LogInformation("Received request to get requisition {RequisitionId}", requisitionId);

        var query = new GetRequisitionQuery { RequisitionId = requisitionId };
        var result = await mediator.Send<GetRequisitionQuery, GetRequisitionQueryResult>(query);

        // Return 404 if requisition not found
        if (result.Requisition is null)
        {
            logger.LogWarning("Requisition {RequisitionId} not found", requisitionId);
            return new NotFoundResult();
        }

        var response = new RequisitionResponse
        {
            Id = result.Requisition.Id,
            Status = result.Requisition.Status.ToString(),
            InstitutionId = result.Requisition.InstitutionId,
            AgreementId = result.Requisition.AgreementId,
            Accounts = result.Requisition.Accounts,
            AuthenticationLink = result.Requisition.AuthenticationLink,
            Reference = result.Requisition.Reference,
        };

        logger.LogInformation("Successfully retrieved requisition {RequisitionId}", requisitionId);
        return new OkObjectResult(response);
    }
}
