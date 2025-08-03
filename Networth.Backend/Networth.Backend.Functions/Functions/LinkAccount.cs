using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Handlers;
using Networth.Backend.Functions.Models.Requests;

namespace Networth.Backend.Functions.Functions;

/// <summary>
///     Azure Function for linking bank accounts by creating agreements and requisitions.
/// </summary>
public class LinkAccount(
    ILinkAccountCommandHandler linkAccountHandler,
    IValidator<LinkAccountRequest> validator,
    ILogger<LinkAccount> logger)
{
    /// <summary>
    ///     Links a bank account by creating an agreement and requisition in sequence.
    /// </summary>
    /// <param name="req">The HTTP request containing link account parameters.</param>
    /// <returns>The created requisition details with authorization link.</returns>
    [Function("LinkAccount")]
    [OpenApiOperation(
        "LinkAccount",
        "Accounts",
        Summary = "Link bank account",
        Description = "Links a bank account by creating an agreement with the financial institution and then creating a requisition for account access.")]
    [OpenApiRequestBody(
        "application/json",
        typeof(LinkAccountRequest),
        Description = "Account linking parameters")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(LinkAccountCommandResult),
        Description = "Successfully linked account")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.BadRequest,
        Description = "Invalid request parameters")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "account/link")]
        HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            LinkAccountRequest? request = JsonSerializer.Deserialize<LinkAccountRequest>(
                requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null)
            {
                logger.LogWarning("Invalid request body for LinkAccount");
                return new BadRequestObjectResult("Invalid request body");
            }

            // Validate the request using FluentValidation
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                logger.LogWarning(
                    "Validation failed for LinkAccount request: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                return new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            LinkAccountCommand command = new()
            {
                InstitutionId = request.InstitutionId,
                RedirectUrl = request.RedirectUrl,
                Reference = request.Reference,
                MaxHistoricalDays = request.MaxHistoricalDays,
                AccessValidForDays = request.AccessValidForDays,
                UserLanguage = request.UserLanguage
            };

            LinkAccountCommandResult result = await linkAccountHandler.HandleAsync(command);

            logger.LogInformation(
                "Successfully linked account for institution {InstitutionId}. Agreement: {AgreementId}, Requisition: {RequisitionId}",
                request.InstitutionId,
                result.Agreement.Id,
                result.Requisition.Id);

            return new OkObjectResult(result);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize LinkAccount request");
            return new BadRequestObjectResult("Invalid JSON format");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error linking account");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
