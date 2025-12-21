using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Requests;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Users;

/// <summary>
///     Azure Function to update user fields.
/// </summary>
public class UpdateUser(
    IMediator mediator,
    ICurrentUserService currentUserService,
    ILogger<UpdateUser> logger)
{
    /// <summary>
    ///     Updates the current user's fields.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>The updated user details.</returns>
    [Function("UpdateUser")]
    [OpenApiOperation(
        "UpdateUser",
        "Users",
        Summary = "Update user fields",
        Description = "Updates the current user's fields such as name and onboarding status.")]
    [OpenApiRequestBody(
        "application/json",
        typeof(UpdateUserRequest),
        Description = "The fields to update")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(UpdateUserResponse),
        Description = "User updated successfully")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Unauthorized,
        Description = "User is not authenticated")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.NotFound,
        Description = "User not found")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/me")]
        HttpRequest req)
    {
        if (!currentUserService.IsAuthenticated)
        {
            logger.LogWarning("Unauthenticated request to UpdateUser");
            return new UnauthorizedResult();
        }

        UpdateUserRequest? request;
        try
        {
            request = await req.ReadFromJsonAsync<UpdateUserRequest>();
        }
        catch
        {
            return new BadRequestObjectResult("Invalid request body");
        }

        if (request == null)
        {
            return new BadRequestObjectResult("Request body is required");
        }

        var command = new UpdateUserCommand
        {
            UserId = currentUserService.InternalUserId!.Value,
            Name = request.Name,
            HasCompletedOnboarding = request.HasCompletedOnboarding,
        };

        try
        {
            var result = await mediator.Send<UpdateUserCommand, UpdateUserCommandResult>(command);

            return new OkObjectResult(new UpdateUserResponse
            {
                UserId = result.UserId,
                FirebaseUid = result.FirebaseUid,
                Name = result.Name,
                HasCompletedOnboarding = result.HasCompletedOnboarding,
            });
        }
        catch (InvalidOperationException)
        {
            return new NotFoundResult();
        }
    }
}
