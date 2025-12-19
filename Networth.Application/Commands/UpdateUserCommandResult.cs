namespace Networth.Application.Commands;

/// <summary>
///     Result of updating a user.
/// </summary>
public class UpdateUserCommandResult
{
    /// <summary>
    ///     Gets or sets the user ID.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user's name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }
}
