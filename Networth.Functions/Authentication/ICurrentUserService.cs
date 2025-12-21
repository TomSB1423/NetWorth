using System.Security.Claims;

namespace Networth.Functions.Authentication;

/// <summary>
///     Service for accessing the current authenticated user.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    ///     Gets the current user's Firebase UID (external identity from auth token).
    /// </summary>
    string FirebaseUid { get; }

    /// <summary>
    ///     Gets the current user's display name.
    /// </summary>
    string? Name { get; }

    /// <summary>
    ///     Gets the current user's email address.
    /// </summary>
    string? Email { get; }

    /// <summary>
    ///     Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    ///     Gets the current user principal.
    /// </summary>
    ClaimsPrincipal? User { get; }

    /// <summary>
    ///     Gets the current user's internal database ID.
    ///     This requires the user to exist in the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The internal user ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user is not found in database.</exception>
    Task<Guid> GetInternalUserIdAsync(CancellationToken cancellationToken = default);
}

