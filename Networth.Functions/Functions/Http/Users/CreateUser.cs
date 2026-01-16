using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Functions.Authentication;
using Networth.Functions.Models.Responses;

namespace Networth.Functions.Functions.Http.Users;

/// <summary>
///     Azure Function to create or ensure a user exists.
/// </summary>
public class CreateUser(
    IMediator mediator,
    ICurrentUserService currentUserService,
    ILogger<CreateUser> logger)
{
    /// <summary>
    ///     Creates a new user or returns the existing user if already present.
    ///     This should be called by the frontend after successful authentication.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>The created or existing user details.</returns>
    [Function("CreateUser")]
    [OpenApiOperation(
        "CreateUser",
        "Users",
        Summary = "Create or ensure user exists",
        Description = "Creates a new user record or returns the existing user. Call this after successful authentication to ensure the user exists in the system.")]
    [OpenApiResponseWithBody(
        HttpStatusCode.OK,
        "application/json",
        typeof(CreateUserResponse),
        Description = "User created or already exists")]
    [OpenApiResponseWithoutBody(
        HttpStatusCode.Unauthorized,
        Description = "User is not authenticated")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        HttpRequest req)
    {
        if (!currentUserService.IsAuthenticated)
        {
            logger.LogWarning("Unauthenticated request to CreateUser");
            return new UnauthorizedResult();
        }

        var command = new CreateUserCommand
        {
            FirebaseUid = currentUserService.FirebaseUid,
            Name = currentUserService.Name!,
            Email = currentUserService.Email!,
        };

        var result = await mediator.Send<CreateUserCommand, CreateUserCommandResult>(command);

        return new OkObjectResult(new CreateUserResponse
        {
            UserId = result.UserId,
            Name = result.Name,
            IsNewUser = result.IsNewUser,
            HasCompletedOnboarding = result.HasCompletedOnboarding,
        });
    }
}
