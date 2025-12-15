using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Networth.Functions.Authentication;

/// <summary>
///     Implementation of <see cref="ICurrentUserService"/> for Azure Functions.
///     Supports JWT Bearer token authentication via custom middleware
///     and falls back to mock users in development.
/// </summary>
public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    IFunctionContextAccessor functionContextAccessor,
    IEnumerable<ClaimsPrincipal> principals) : ICurrentUserService
{
    private readonly ClaimsPrincipal? _injectedPrincipal = principals.FirstOrDefault();

    /// <inheritdoc />
    public ClaimsPrincipal? User
    {
        get
        {
            // First, try to get the user from FunctionContext.Items
            // This is set by the JwtAuthenticationMiddleware after token validation
            if (functionContextAccessor.FunctionContext?.Items.TryGetValue("User", out var contextUser) == true
                && contextUser is ClaimsPrincipal principal
                && principal.Identity?.IsAuthenticated == true)
            {
                return principal;
            }

            // Fall back to HttpContext.User
            var httpUser = httpContextAccessor.HttpContext?.User;
            if (httpUser?.Identity?.IsAuthenticated == true)
            {
                return httpUser;
            }

            // Fall back to injected mock user (development only)
            return _injectedPrincipal ?? httpUser;
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public string UserId =>
        // JWT tokens from Entra ID use 'oid' (object ID) or 'sub' (subject) for user identification
        User?.FindFirst("oid")?.Value
        ?? User?.FindFirst("sub")?.Value
        ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User is not authenticated");

    /// <inheritdoc />
    public string? Name =>
        User?.FindFirst("name")?.Value
        ?? User?.FindFirst(ClaimTypes.Name)?.Value;
}

