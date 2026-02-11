namespace Networth.Functions.Models.Requests;

/// <summary>
///     Request model for updating a user.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    ///     Gets or sets the user's display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets whether the user has completed onboarding.
    /// </summary>
    public bool? HasCompletedOnboarding { get; set; }
}
