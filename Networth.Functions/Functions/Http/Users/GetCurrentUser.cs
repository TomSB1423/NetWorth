using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Networth.Domain.Repositories;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Users;

/// <summary>
///     Azure Function for retrieving the current authenticated user information.
/// </summary>
public class GetCurrentUser(
    ICurrentUserService currentUserService,
    IUserRepository userRepository)
{
    /// <summary>
    ///     Gets the current authenticated user information from the database.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>The current user information.</returns>
    [Function("GetCurrentUser")]
    [OpenApiOperation("GetCurrentUser", ["Users"], Summary = "Get current authenticated user", Description = "Returns the current user's profile from the database")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CurrentUserResponse), Description = "Current user profile")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Unauthorized, Description = "User is not authenticated")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Description = "User has not been created yet")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/me")] HttpRequest req)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return new UnauthorizedResult();
        }

        var user = await userRepository.GetUserByFirebaseUidAsync(currentUserService.FirebaseUid);

        if (user == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(new CurrentUserResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            HasCompletedOnboarding = user.HasCompletedOnboarding,
        });
    }
}
