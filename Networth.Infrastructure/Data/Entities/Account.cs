using Networth.Domain.Enums;
using AccountCategory = Networth.Domain.Enums.AccountCategory;

namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a bank account.
/// </summary>
public class Account
{
    /// <summary>
    ///     Gets or sets the unique identifier for the account.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the owner user ID.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the requisition ID that created this account.
    /// </summary>
    public required string RequisitionId { get; set; }

    /// <summary>
    ///     Gets or sets the institution metadata ID.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the account.
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
    ///     Gets or sets the IBAN of the account.
    /// </summary>
    public string? Iban { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the account.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    ///     Gets or sets the product name/type.
    /// </summary>
    public string? Product { get; set; }

    /// <summary>
    ///     Gets or sets the cash account type (e.g., CACC).
    /// </summary>
    public string? CashAccountType { get; set; }

    /// <summary>
    ///     Gets or sets the status of the account.
    /// </summary>
    public AccountLinkStatus Status { get; set; } = AccountLinkStatus.Linked;

    /// <summary>
    ///     Gets or sets additional account data as JSON.
    /// </summary>
    public string? AdditionalAccountData { get; set; }

    /// <summary>
    ///     Gets or sets when this account was created.
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or sets when this account was last synced.
    /// </summary>
    public DateTime? LastSynced { get; set; }

    /// <summary>
    ///     Gets or sets the transactions associated with this account.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];

    /// <summary>
    ///     Gets or sets the balances associated with this account.
    /// </summary>
    public ICollection<AccountBalance> Balances { get; set; } = [];
}
