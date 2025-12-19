namespace Networth.Application.Commands;

/// <summary>
///     Result of the create user command.
/// </summary>
public class CreateUserCommandResult
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
    ///     Gets or sets a value indicating whether the user was newly created (true) or already existed (false).
    /// </summary>
    public bool IsNewUser { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }
}
