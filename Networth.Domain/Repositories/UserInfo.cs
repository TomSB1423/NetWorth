namespace Networth.Domain.Repositories;

/// <summary>
///     Represents a user in the system.
/// </summary>
public class UserInfo
{
    /// <summary>
    ///     Gets or sets the internal unique identifier for the user.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase UID from the identity provider.
    /// </summary>
    public required string FirebaseUid { get; set; }

    /// <summary>
    ///     Gets or sets the display name of the user.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }
}
