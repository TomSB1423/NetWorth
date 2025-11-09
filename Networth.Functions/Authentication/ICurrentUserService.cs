using System.Security.Claims;

namespace Networth.Functions.Authentication;

/// <summary>
///     Service for accessing the current authenticated user.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    ///     Gets the current user's ID.
    /// </summary>
    string UserId { get; }

    /// <summary>
    ///     Gets the current user's display name.
    /// </summary>
    string? Name { get; }

    /// <summary>
    ///     Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    ///     Gets the current user principal.
    /// </summary>
    ClaimsPrincipal? User { get; }
}

