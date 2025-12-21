using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Institutions;

/// <summary>
///     Azure Function for retrieving available financial institutions.
/// </summary>
public class GetInstitutions(
    IMediator mediator,
    ICurrentUserService currentUserService,
    ILogger<GetInstitutions> logger)
{
    /// <summary>
    ///     Gets a list of available financial institutions.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>A list of institutions.</returns>
    [Function("GetInstitutions")]
    [OpenApiOperation(
        "GetInstitutions",
        "Institutions",
        Summary = "Get financial institutions",
        Description = "Retrieves a list of available financial institutions for account linking. Already linked institutions are excluded.")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(IEnumerable<InstitutionResponse>),
        Description = "Successfully retrieved institutions")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "institutions")]
        HttpRequest req)
    {
        logger.LogInformation("Received request to get institutions");

        Guid? userId = null;
        if (currentUserService.IsAuthenticated)
        {
            userId = await currentUserService.GetInternalUserIdAsync();
        }

        var query = new GetInstitutionsQuery
        {
            CountryCode = "GB",
            UserId = userId,
            ExcludeLinked = true,
        };
        var result = await mediator.Send<GetInstitutionsQuery, GetInstitutionsQueryResult>(query);

        var response = result.Institutions.Select(i => new InstitutionResponse
        {
            Id = i.Id,
            Name = i.Name,
            LogoUrl = i.LogoUrl,
        });

        logger.LogInformation("Successfully retrieved {Count} institutions", response.Count());
        return new OkObjectResult(response);
    }
}
