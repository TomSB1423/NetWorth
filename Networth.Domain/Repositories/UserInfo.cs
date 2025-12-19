namespace Networth.Domain.Repositories;

/// <summary>
///     Represents a user in the system.
/// </summary>
public class UserInfo
{
    /// <summary>
    ///     Gets or sets the unique identifier for the user.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the display name of the user.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }
}
