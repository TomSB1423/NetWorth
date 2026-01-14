using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Networth.Functions.Authentication;

namespace Networth.Functions.Middleware;

/// <summary>
///     Middleware that injects a mock user for all requests in development.
///     Replaces JWT authentication when MockAuthentication=true.
///     This middleware should only be registered in development with mock auth enabled.
/// </summary>
public class MockUserMiddleware(ILogger<MockUserMiddleware> logger) : IFunctionsWorkerMiddleware
{
    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpContext = context.GetHttpContext();
        if (httpContext is null)
        {
            await next(context);
            return;
        }

        // Always inject the default mock user
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, MockUser.FirebaseUid),
            new("sub", MockUser.FirebaseUid),
            new(ClaimTypes.Name, MockUser.Name),
            new("name", MockUser.Name),
            new(ClaimTypes.Email, MockUser.Email),
            new("email", MockUser.Email),
        };

        var identity = new ClaimsIdentity(claims, "MockAuthentication");
        var principal = new ClaimsPrincipal(identity);

        httpContext.User = principal;
        context.Items["User"] = principal;

        logger.LogDebug("Mock user injected: {MockUserId}", MockUser.FirebaseUid);

        await next(context);
    }
}
