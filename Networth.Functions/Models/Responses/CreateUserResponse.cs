namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for user creation.
/// </summary>
public class CreateUserResponse
{
    /// <summary>
    ///     Gets or sets the user ID.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user's display name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this is a newly created user.
    /// </summary>
    public bool IsNewUser { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }
}
