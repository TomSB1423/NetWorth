namespace Networth.Application.Commands;

/// <summary>
///     Command to update user fields.
/// </summary>
public class UpdateUserCommand
{
    /// <summary>
    ///     Gets or sets the internal user ID.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user's display name (optional update).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets whether the user has completed onboarding (optional update).
    /// </summary>
    public bool? HasCompletedOnboarding { get; set; }
}
