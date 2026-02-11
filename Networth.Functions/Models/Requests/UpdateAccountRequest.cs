using Networth.Domain.Enums;

namespace Networth.Functions.Models.Requests;

/// <summary>
///     Request model for updating an account.
/// </summary>
public class UpdateAccountRequest
{
    /// <summary>
    ///     Gets or sets the user-defined display name for the account.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    ///     Gets or sets the user-specified category for the account.
    /// </summary>
    public AccountCategory? Category { get; set; }
}
