using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;
using Networth.Backend.Functions.Models.Requests;

namespace Networth.Backend.Functions.Functions;

/// <summary>
///     Azure Function for creating requisitions for bank account access.
/// </summary>
public class CreateRequisition(IFinancialProvider financialProvider, ILogger<CreateRequisition> logger)
{
    /// <summary>
    ///     Creates a new requisition for accessing bank accounts.
    /// </summary>
    /// <param name="req">The HTTP request containing requisition parameters.</param>
    /// <returns>The created requisition details with authorization link.</returns>
    [Function("CreateRequisition")]
    [OpenApiOperation(
        "CreateRequisition",
        "Requisitions",
        Summary = "Create requisition",
        Description = "Creates a new requisition for accessing bank accounts through a financial institution.")]
    [OpenApiRequestBody(
        "application/json",
        typeof(CreateRequisitionRequest),
        Description = "Requisition creation parameters")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(Requisition),
        Description = "Successfully created requisition")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid request parameters")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "requisitions")]
        HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateRequisitionRequest? request = JsonSerializer.Deserialize<CreateRequisitionRequest>(
                requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null)
            {
                logger.LogWarning("Invalid request body for CreateRequisition");
                return new BadRequestObjectResult("Invalid request body");
            }

            if (string.IsNullOrEmpty(request.RedirectUrl) ||
                string.IsNullOrEmpty(request.InstitutionId) ||
                string.IsNullOrEmpty(request.AgreementId) ||
                string.IsNullOrEmpty(request.Reference))
            {
                logger.LogWarning("Missing required fields in CreateRequisition request");
                return new BadRequestObjectResult("RedirectUrl, InstitutionId, AgreementId, and Reference are required");
            }

            Requisition requisition = await financialProvider.CreateRequisitionAsync(
                request.RedirectUrl,
                request.InstitutionId,
                request.AgreementId,
                request.Reference,
                request.UserLanguage);

            logger.LogInformation(
                "Successfully created requisition {RequisitionId} for institution {InstitutionId}",
                requisition.Id,
                request.InstitutionId);

            return new OkObjectResult(requisition);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize CreateRequisition request");
            return new BadRequestObjectResult("Invalid JSON format");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating requisition");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
