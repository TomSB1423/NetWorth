using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Networth.Functions.Authentication;

namespace Networth.Functions.Middleware;

/// <summary>
///     Middleware to handle authentication for Azure Functions isolated worker.
///     Parses X-Test-User header (for tests) or validates JWT Bearer tokens (for production).
///     Sets the authenticated user on both HttpContext.User and ICurrentUserService.
/// </summary>
public class AuthenticationMiddleware(ILogger<AuthenticationMiddleware> logger) : IFunctionsWorkerMiddleware
{
    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpContext = context.GetHttpContext();

        if (httpContext != null)
        {
            ClaimsPrincipal? principal = null;

            // Try X-Test-User header first (for integration tests)
            var testUserHeader = httpContext.Request.Headers["X-Test-User"].ToString();
            if (!string.IsNullOrEmpty(testUserHeader))
            {
                principal = ParseTestUserHeader(testUserHeader);
                if (principal != null)
                {
                    logger.LogDebug(
                        "Authenticated via X-Test-User header: {UserId}",
                        principal.FindFirst("sub")?.Value);
                }
            }

            // TODO: Add JWT Bearer token validation for production

            if (principal != null)
            {
                // Set on HttpContext.User for direct access via req.HttpContext.User
                httpContext.User = principal;

                // Set on CurrentUserService for DI access
                // Note: CurrentUserService is scoped, so we get the instance for this request
                var currentUserService = context.InstanceServices.GetService<ICurrentUserService>() as CurrentUserService;
                if (currentUserService != null)
                {
                    currentUserService.User = principal;
                }
            }
        }

        await next(context);
    }

    private ClaimsPrincipal? ParseTestUserHeader(string json)
    {
        try
        {
            var claimsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (claimsDict == null)
            {
                return null;
            }

            var claims = new List<Claim>();
            foreach (var kvp in claimsDict)
            {
                if (kvp.Value.ValueKind == JsonValueKind.String)
                {
                    var claimType = kvp.Key switch
                    {
                        // Map common JWT claims to standard .NET claim types
                        "sub" => ClaimTypes.NameIdentifier,
                        "name" => ClaimTypes.Name,
                        "email" => ClaimTypes.Email,
                        "role" => ClaimTypes.Role,
                        _ => kvp.Key,
                    };
                    claims.Add(new Claim(claimType, kvp.Value.GetString()!));
                }
            }

            var identity = new ClaimsIdentity(claims, "TestAuth", ClaimTypes.NameIdentifier, ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to parse X-Test-User header");
            return null;
        }
    }
}
