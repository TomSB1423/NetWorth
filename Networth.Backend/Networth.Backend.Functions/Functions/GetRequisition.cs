using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Functions.Functions;

/// <summary>
///     Azure Function for retrieving requisition details.
/// </summary>
public class GetRequisition(IFinancialProvider financialProvider, ILogger<GetRequisition> logger)
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
        In = Microsoft.OpenApi.Models.ParameterLocation.Path,
        Required = true,
        Type = typeof(string),
        Description = "The requisition ID")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(Requisition),
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
        try
        {
            if (string.IsNullOrEmpty(requisitionId))
            {
                logger.LogWarning("Missing requisitionId in GetRequisition request");
                return new BadRequestObjectResult("Requisition ID is required");
            }

            Requisition requisition = await financialProvider.GetRequisitionAsync(requisitionId);

            logger.LogInformation("Successfully retrieved requisition {RequisitionId}", requisitionId);

            return new OkObjectResult(requisition);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving requisition {RequisitionId}", requisitionId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
