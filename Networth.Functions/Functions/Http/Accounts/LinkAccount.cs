using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Functions.Models.Requests;
using Networth.Functions.Models.Responses;
using FromBodyAttributes = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Networth.Functions.Functions.Http.Accounts;

/// <summary>
///     Azure Function for linking bank accounts by creating agreements and requisitions.
/// </summary>
public class LinkAccount(IMediator mediator)
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
        typeof(LinkAccountResponse),
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
        var command = new LinkAccountCommand
        {
            UserId = request.UserId,
            InstitutionId = request.InstitutionId,
        };
        var result = await mediator.Send<LinkAccountCommand, LinkAccountCommandResult>(command);

        var response = new LinkAccountResponse
        {
            AuthorizationLink = result.AuthorizationLink,
            Status = result.Status,
        };

        return new OkObjectResult(response);
    }
}
