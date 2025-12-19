using Networth.Domain.Enums;

namespace Networth.Application.Commands;

/// <summary>
///     Result of updating an account.
/// </summary>
public class UpdateAccountCommandResult
{
    /// <summary>
    ///     Gets or sets the account ID.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the user ID.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the institution ID.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the institution name.
    /// </summary>
    public string? InstitutionName { get; set; }

    /// <summary>
    ///     Gets or sets the institution logo URL.
    /// </summary>
    public string? InstitutionLogo { get; set; }

    /// <summary>
    ///     Gets or sets the account name from the bank.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the user-defined display name for the account.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    ///     Gets or sets the user-specified category for the account.
    /// </summary>
    public AccountCategory? Category { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the account.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    ///     Gets or sets the product name/type from the bank.
    /// </summary>
    public string? Product { get; set; }
}
