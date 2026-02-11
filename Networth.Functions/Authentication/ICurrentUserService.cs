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
    ///     Gets the current user's internal database ID.
    ///     Returns null if user hasn't been created yet or ID hasn't been resolved.
    /// </summary>
    Guid? InternalUserId { get; }

    /// <summary>
    ///     Gets the current user's display name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when user is not authenticated.</exception>
    string Name { get; }

    /// <summary>
    ///     Gets the current user's email address.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when user is not authenticated.</exception>
    string Email { get; }

    /// <summary>
    ///     Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    ///     Gets the current user principal.
    /// </summary>
    ClaimsPrincipal? User { get; }

    /// <summary>
    ///     Sets the internal user ID. Called by middleware after resolving the user.
    /// </summary>
    /// <param name="userId">The internal user ID.</param>
    void SetInternalUserId(Guid userId);

    /// <summary>
    ///     Gets the current user's internal database ID, resolving it if not already cached.
    ///     This requires the user to exist in the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The internal user ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user is not found in database.</exception>
    Task<Guid> GetInternalUserIdAsync(CancellationToken cancellationToken = default);
}

