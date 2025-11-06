using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Functions.Functions;

public class GetInstitutions(IFinancialProvider financialProvider)
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
        typeof(object),
        Description = "Successfully retrieved institutions")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.InternalServerError,
        Description = "Internal server error")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "institutions")]
        HttpRequest req)
    {
        IEnumerable<InstitutionMetadata> institutions = await financialProvider.GetInstitutionsAsync("GB");
        return new OkObjectResult(institutions);
    }
}
