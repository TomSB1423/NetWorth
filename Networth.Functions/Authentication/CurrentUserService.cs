using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Networth.Domain.Repositories;

namespace Networth.Functions.Authentication;

/// <summary>
///     Implementation of <see cref="ICurrentUserService"/> for Azure Functions.
///     Supports Firebase token authentication via custom middleware
///     and falls back to mock users in development.
/// </summary>
public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    IFunctionContextAccessor functionContextAccessor,
    IUserRepository userRepository,
    IEnumerable<ClaimsPrincipal> principals) : ICurrentUserService
{
    private readonly ClaimsPrincipal? _injectedPrincipal = principals.FirstOrDefault();

    /// <inheritdoc />
    public Guid? InternalUserId { get; private set; }

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
    public string FirebaseUid =>
        // Firebase tokens use 'sub' claim for user identification (Firebase UID)
        User?.FindFirst("sub")?.Value
        ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User is not authenticated");

    /// <inheritdoc />
    public string? Name =>
        User?.FindFirst("name")?.Value
        ?? User?.FindFirst(ClaimTypes.Name)?.Value;

    /// <inheritdoc />
    public string? Email =>
        User?.FindFirst("email")?.Value
        ?? User?.FindFirst(ClaimTypes.Email)?.Value;

    /// <inheritdoc />
    public void SetInternalUserId(Guid userId)
    {
        InternalUserId = userId;
    }

    /// <inheritdoc />
    public async Task<Guid> GetInternalUserIdAsync(CancellationToken cancellationToken = default)
    {
        // Return cached value if available (set by middleware or previous call)
        if (InternalUserId.HasValue)
        {
            return InternalUserId.Value;
        }

        var user = await userRepository.GetUserByFirebaseUidAsync(FirebaseUid, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException($"User with Firebase UID '{FirebaseUid}' not found in database. Ensure the user is created first.");
        }

        InternalUserId = user.Id;
        return user.Id;
    }
}

