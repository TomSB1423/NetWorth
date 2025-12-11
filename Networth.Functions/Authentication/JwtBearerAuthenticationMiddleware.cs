using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace Networth.Functions.Authentication;

/// <summary>
///     Middleware for JWT Bearer token authentication using Microsoft.Identity.Web.
///     Validates access tokens from Microsoft Entra ID and extracts user claims.
/// </summary>
public class JwtBearerAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    /// <summary>
    ///     Invokes the middleware to validate JWT Bearer tokens and extract user information.
    /// </summary>
    /// <param name="context">The function context.</param>
    /// <param name="next">The next middleware in the pipeline.</param>
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var logger = context.GetLogger<JwtBearerAuthenticationMiddleware>();

        // Try to get HttpContext (ASP.NET Core integration)
        var httpContext = context.GetHttpContext();

        if (httpContext == null)
        {
            logger.LogError("HttpContext is null. Cannot authenticate.");
            return;
        }

        string? authHeader = null;
        if (httpContext.Request.Headers.TryGetValue("Authorization", out var headerValues))
        {
            authHeader = headerValues.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("No valid authorization header found. Returning Unauthorized.");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        string token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            // Get the token validation service from DI
            var tokenValidationService = context.InstanceServices.GetService<ITokenValidationService>();
            if (tokenValidationService == null)
            {
                throw new InvalidOperationException("Token validation service not configured.");
            }

            // Validate the token
            ClaimsPrincipal principal = await tokenValidationService.ValidateTokenAsync(token);

            // Store the principal in the function context
            context.Items["User"] = principal;

            // Set the context on the current user service
            ICurrentUserService? currentUserService = context.InstanceServices.GetService<ICurrentUserService>();
            if (currentUserService is CurrentUserService service)
            {
                service.SetContext(context);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Token validation failed");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }
}
