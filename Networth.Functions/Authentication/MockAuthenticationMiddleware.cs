using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Networth.Functions.Authentication;

/// <summary>
///     Middleware for mock authentication in development.
///     Provides a fake authenticated user for testing purposes.
///     Creates a mock user with ID "mock-user-123" which corresponds to a user in the database.
/// </summary>
/// <remarks>
///     This middleware is deprecated. Authentication is now handled via:
///     - Production: Easy Auth (App Service Authentication) with Microsoft.Identity.Web.
///     - Development: Mock ClaimsPrincipal registered in DI (Program.cs).
/// </remarks>
[Obsolete("Use Microsoft.Identity.Web AppServicesAuthentication instead. Mock users are registered via DI in Program.cs.")]
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
                new Claim(ClaimTypes.Name, "Mock Development User"),
                new Claim("IsActive", "true"),
            ],
            "MockAuthentication");

        ClaimsPrincipal principal = new(identity);

        // Store the principal in the function context for any downstream consumers
        context.Items["User"] = principal;

        await next(context);
    }
}

