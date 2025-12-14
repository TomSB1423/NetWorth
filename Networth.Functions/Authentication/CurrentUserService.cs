using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Networth.Functions.Authentication;

/// <summary>
///     Implementation of <see cref="ICurrentUserService"/> for Azure Functions.
///     Supports Easy Auth (App Service Authentication) via Microsoft.Identity.Web
///     and falls back to mock users in development.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor, IEnumerable<ClaimsPrincipal> principals) : ICurrentUserService
{
    private readonly ClaimsPrincipal? _injectedPrincipal = principals.FirstOrDefault();

    /// <inheritdoc />
    public ClaimsPrincipal? User
    {
        get
        {
            // In production with Easy Auth, Microsoft.Identity.Web's AppServicesAuthentication
            // handler populates HttpContext.User from the X-MS-CLIENT-PRINCIPAL header
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
    public string UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User is not authenticated");

    /// <inheritdoc />
    public string? Name => User?.FindFirst(ClaimTypes.Name)?.Value;
}

