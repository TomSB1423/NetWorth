using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Networth.Backend.Functions.Authentication;

/// <summary>
///     Middleware for mock authentication in development.
///     Provides a fake authenticated user for testing purposes.
/// </summary>
public class MockAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    /// <summary>
    ///     Invokes the middleware to add mock authentication.
    /// </summary>
    /// <param name="context">The function context.</param>
    /// <param name="next">The next middleware in the pipeline.</param>
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // Create mock user identity
        ClaimsIdentity identity = new(
            [
                new Claim(ClaimTypes.NameIdentifier, "mock-user-123"),
                new Claim(ClaimTypes.Email, "mockuser@networth.dev"),
                new Claim(ClaimTypes.Name, "Mock Development User"),
                new Claim("IsActive", "true")
            ],
            "MockAuthentication");

        ClaimsPrincipal principal = new(identity);

        // Store the principal in the function context
        context.Items["User"] = principal;

        await next(context);
    }
}

