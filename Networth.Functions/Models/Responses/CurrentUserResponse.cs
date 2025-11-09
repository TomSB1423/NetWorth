namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for current user information.
/// </summary>
public class CurrentUserResponse
{
    /// <summary>
    ///     Gets or sets the user ID.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated { get; set; }
}
