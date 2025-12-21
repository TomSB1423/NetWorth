namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for current user information.
/// </summary>
public class CurrentUserResponse
{
    /// <summary>
    ///     Gets or sets the internal user ID.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase UID from the authentication token.
    /// </summary>
    public required string FirebaseUid { get; set; }

    /// <summary>
    ///     Gets or sets the user's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }
}
