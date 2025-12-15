namespace Networth.Application.Commands;

/// <summary>
///     Command to create or ensure a user exists in the system.
/// </summary>
public class CreateUserCommand
{
    /// <summary>
    ///     Gets or sets the user ID from the authentication token.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user's display name.
    /// </summary>
    public string? Name { get; set; }
}

