using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Institutions;

/// <summary>
///     Azure Function for retrieving available financial institutions.
/// </summary>
[Authorize]
public class GetInstitutions(IMediator mediator, IHostEnvironment environment, ILogger<GetInstitutions> logger)
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
        Description = "Retrieves a list of available financial institutions for account linking.")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(IEnumerable<InstitutionResponse>),
        Description = "Successfully retrieved institutions")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "institutions")]
        HttpRequest req)
    {
        logger.LogInformation("Received request to get institutions");

        var query = new GetInstitutionsQuery
        {
            CountryCode = "GB",
            IncludeSandbox = environment.IsDevelopment(),
        };
        var result = await mediator.Send<GetInstitutionsQuery, GetInstitutionsQueryResult>(query);

        var response = result.Institutions.Select(i => new InstitutionResponse(
            i.Id,
            i.Name,
            i.LogoUrl));

        logger.LogInformation("Successfully retrieved {Count} institutions", response.Count());
        return new OkObjectResult(response);
    }
}
