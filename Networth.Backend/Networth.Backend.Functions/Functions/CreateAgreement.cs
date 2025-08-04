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
///     Azure Function for creating agreements with financial institutions.
/// </summary>
public class CreateAgreement(IFinancialProvider financialProvider, ILogger<CreateAgreement> logger)
{
    /// <summary>
    ///     Creates a new agreement with a financial institution.
    /// </summary>
    /// <param name="req">The HTTP request containing agreement parameters.</param>
    /// <returns>The created agreement details.</returns>
    [Function("CreateAgreement")]
    [OpenApiOperation(
        "CreateAgreement",
        "Agreements",
        Summary = "Create agreement",
        Description = "Creates a new agreement with a financial institution for accessing account data.")]
    [OpenApiRequestBody(
        "application/json",
        typeof(CreateAgreementRequest),
        Description = "Agreement creation parameters")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(Agreement),
        Description = "Successfully created agreement")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid request parameters")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "agreements")]
        HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateAgreementRequest? request = JsonSerializer.Deserialize<CreateAgreementRequest>(
                requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null)
            {
                logger.LogWarning("Invalid request body for CreateAgreement");
                return new BadRequestObjectResult("Invalid request body");
            }

            if (string.IsNullOrEmpty(request.InstitutionId))
            {
                logger.LogWarning("Missing InstitutionId in CreateAgreement request");
                return new BadRequestObjectResult("InstitutionId is required");
            }

            Agreement agreement = await financialProvider.CreateAgreementAsync(
                request.InstitutionId,
                request.MaxHistoricalDays,
                request.AccessValidForDays);

            logger.LogInformation(
                "Successfully created agreement {AgreementId} for institution {InstitutionId}",
                agreement.Id,
                request.InstitutionId);

            return new OkObjectResult(agreement);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize CreateAgreement request");
            return new BadRequestObjectResult("Invalid JSON format");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating agreement");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
