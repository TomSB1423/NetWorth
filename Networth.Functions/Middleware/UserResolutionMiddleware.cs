using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Networth.Domain.Repositories;
using Networth.Functions.Authentication;

namespace Networth.Functions.Middleware;

/// <summary>
///     Middleware that resolves the internal user ID from the database after JWT authentication.
///     Returns 404 if user is authenticated but doesn't exist in the database.
///     This allows endpoints to access <see cref="ICurrentUserService.InternalUserId"/> synchronously.
/// </summary>
public class UserResolutionMiddleware(ILogger<UserResolutionMiddleware> logger) : IFunctionsWorkerMiddleware
{
    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var currentUserService = context.InstanceServices.GetRequiredService<ICurrentUserService>();
        var userRepository = context.InstanceServices.GetRequiredService<IUserRepository>();

        // Only resolve if user is authenticated and ID not already set
        if (currentUserService.IsAuthenticated && !currentUserService.InternalUserId.HasValue)
        {
            var user = await userRepository.GetUserByFirebaseUidAsync(currentUserService.FirebaseUid);
            if (user is null)
            {
                logger.LogWarning(
                    "User with Firebase UID {FirebaseUid} not found in database",
                    currentUserService.FirebaseUid);

                var httpContext = context.GetHttpContext();
                if (httpContext is not null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    await httpContext.Response.WriteAsync("User not found. Please create your account first.");
                    return;
                }
            }

            currentUserService.SetInternalUserId(user!.Id);
            logger.LogDebug(
                "Resolved internal user ID {UserId} for Firebase UID {FirebaseUid}",
                user.Id,
                currentUserService.FirebaseUid);
        }

        await next(context);
    }
}
