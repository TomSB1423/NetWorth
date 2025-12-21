using Networth.Domain.Enums;

namespace Networth.Application.Commands;

/// <summary>
///     Command to update an account's user-defined fields.
/// </summary>
public class UpdateAccountCommand
{
    /// <summary>
    ///     Gets or sets the account ID.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the internal user ID.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user-defined display name for the account.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    ///     Gets or sets the user-specified category for the account.
    /// </summary>
    public AccountCategory? Category { get; set; }
}
