using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Interfaces;

namespace Networth.Backend.Functions.Functions;

public class GetInstitutions()
{
    /// <summary>
    /// Gets a list of institutions.
    /// </summary>
    [Function("GetInstitutions")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
        [FromServices] IFinancialProvider financialProvider)
    {
        var institutions = await financialProvider.GetInstitutionsAsync();
        return new OkObjectResult(institutions);
    }
}
