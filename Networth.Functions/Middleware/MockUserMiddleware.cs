using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Networth.Functions.Options;

namespace Networth.Functions.Middleware;

/// <summary>
///     Middleware that injects a mock user for all requests in development.
///     Replaces JWT authentication when UseAuthentication=false.
///     This middleware should only be registered in development with mock auth enabled.
/// </summary>
public class MockUserMiddleware(
    ILogger<MockUserMiddleware> logger,
    IOptions<NetworthOptions> options) : IFunctionsWorkerMiddleware
{
    private readonly MockUserOptions _mockUser = options.Value.MockUser;

    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpContext = context.GetHttpContext();
        if (httpContext is null)
        {
            await next(context);
            return;
        }

        // Inject the configured mock user
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _mockUser.FirebaseUid!),
            new("sub", _mockUser.FirebaseUid!),
            new(ClaimTypes.Name, _mockUser.Name!),
            new("name", _mockUser.Name!),
            new(ClaimTypes.Email, _mockUser.Email!),
            new("email", _mockUser.Email!),
        };

        var identity = new ClaimsIdentity(claims, "MockAuthentication");
        var principal = new ClaimsPrincipal(identity);

        httpContext.User = principal;
        context.Items["User"] = principal;

        logger.LogDebug("Mock user injected: {MockUserId}", _mockUser.FirebaseUid);

        await next(context);
    }
}
