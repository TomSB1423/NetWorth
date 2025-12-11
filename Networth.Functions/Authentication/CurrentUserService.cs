using System.Security.Claims;

namespace Networth.Functions.Authentication;

/// <summary>
///     Implementation of <see cref="ICurrentUserService"/> for Azure Functions.
///     The User property is set by the AuthenticationMiddleware.
///     Note: IHttpContextAccessor doesn't work in Azure Functions isolated worker,
///     so we use a scoped service pattern where middleware sets the User.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    /// <inheritdoc />
    public ClaimsPrincipal? User { get; set; }

    /// <inheritdoc />
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public string UserId =>
        User?.FindFirst("sub")?.Value
        ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User is not authenticated");

    /// <inheritdoc />
    public string? Name =>
        User?.FindFirst("name")?.Value
        ?? User?.FindFirst(ClaimTypes.Name)?.Value;
}

