namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for user updates.
/// </summary>
public class UpdateUserResponse
{
    /// <summary>
    ///     Gets or sets the internal user ID.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user's display name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }
}
