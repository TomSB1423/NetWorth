using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Users;

/// <summary>
///     Azure Function for retrieving the current authenticated user information.
/// </summary>
public class GetCurrentUser
{
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GetCurrentUser"/> class.
    /// </summary>
    /// <param name="currentUserService">The current user service.</param>
    public GetCurrentUser(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    ///     Gets the current authenticated user information.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>The current user information.</returns>
    [Function("GetCurrentUser")]
    [OpenApiOperation("GetCurrentUser", ["User"], Summary = "Get current authenticated user", Description = "Returns information about the currently authenticated user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CurrentUserResponse), Description = "Current user information")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/me")] HttpRequest req)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            return new UnauthorizedResult();
        }

        CurrentUserResponse response = new()
        {
            UserId = _currentUserService.UserId,
            Name = _currentUserService.Name,
            IsAuthenticated = _currentUserService.IsAuthenticated,
        };

        return new OkObjectResult(response);
    }
}

