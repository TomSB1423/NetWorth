using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Handlers;
using Networth.Backend.Functions.Models.Requests;
using FromBodyAttributes = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Networth.Backend.Functions.Functions;

/// <summary>
///     Azure Function for linking bank accounts by creating agreements and requisitions.
/// </summary>
public class LinkAccount(LinkAccountCommandHandler linkAccountHandler)
{
    /// <summary>
    ///     Links a bank account by creating an agreement and requisition in sequence.
    /// </summary>
    /// <param name="request">The HTTP request containing link account parameters.</param>
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
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "account/link")] [FromBodyAttributes]
        LinkAccountRequest request)
    {
        LinkAccountCommand command = new() { InstitutionId = request.InstitutionId };
        LinkAccountCommandResult result = await linkAccountHandler.HandleAsync(command);
        return new OkObjectResult(result);
    }
}
