using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Users;

/// <summary>
///     Azure Function for retrieving the current authenticated user information.
/// </summary>
[AllowAnonymous]
public class GetCurrentUser(ILogger<GetCurrentUser> logger)
{
    /// <summary>
    ///     Gets the current authenticated user information.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>The current user information.</returns>
    [Function("GetCurrentUser")]
    [AllowAnonymous]
    [OpenApiOperation("GetCurrentUser", ["User"], Summary = "Get current authenticated user", Description = "Returns information about the currently authenticated user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CurrentUserResponse), Description = "Current user information")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/me")] HttpRequest req)
    {
        // In Azure Functions isolated worker with ASP.NET Core integration,
        // IHttpContextAccessor.HttpContext is null, but req.HttpContext contains the correct user
        // that was set by our AuthenticationMiddleware.
        var user = req.HttpContext.User;
        var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

        logger.LogDebug(
            "GetCurrentUser called. IsAuthenticated: {IsAuthenticated}, User: {User}",
            isAuthenticated,
            user?.Identity?.Name);

        if (!isAuthenticated)
        {
            return new UnauthorizedResult();
        }

        var userId = user!.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User is authenticated but has no NameIdentifier claim");
        var name = user.FindFirst(ClaimTypes.Name)?.Value;

        CurrentUserResponse response = new()
        {
            UserId = userId,
            Name = name,
            IsAuthenticated = isAuthenticated,
        };

        return new OkObjectResult(response);
    }
}

