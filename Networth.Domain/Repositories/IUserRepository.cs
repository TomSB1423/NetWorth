namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for User entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    ///     Gets a user by their ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user info, or null if not found.</returns>
    Task<UserInfo?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new user or returns the existing user if already present.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="name">The user's display name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the user info and whether the user was newly created.</returns>
    Task<(UserInfo User, bool IsNew)> CreateOrGetUserAsync(string userId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a user's fields.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="name">The new name, or null to keep existing.</param>
    /// <param name="hasCompletedOnboarding">The new onboarding status, or null to keep existing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user info, or null if user not found.</returns>
    Task<UserInfo?> UpdateUserAsync(
        string userId,
        string? name,
        bool? hasCompletedOnboarding,
        CancellationToken cancellationToken = default);
}
