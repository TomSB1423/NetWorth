namespace Networth.Application.Commands;

/// <summary>
///     Command to create or ensure a user exists in the system.
/// </summary>
public class CreateUserCommand
{
    /// <summary>
    ///     Gets or sets the Firebase UID from the authentication token.
    /// </summary>
    public required string FirebaseUid { get; set; }

    /// <summary>
    ///     Gets or sets the user's display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }
}

