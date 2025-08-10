namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents a bank account with its metadata according to GoCardless API.
/// </summary>
public class Account
{
    /// <summary>
    ///     Gets or sets the unique identifier for the bank account.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of account.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the institution id associated with this account.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the processing status of this account.
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the bank account.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    ///     Gets or sets the account type.
    /// </summary>
    public BankAccountType? AccountType { get; set; }
}
