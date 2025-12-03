namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a snapshot of an account's balance at a specific point in time.
/// </summary>
public class AccountBalance
{
    /// <summary>
    ///     Gets or sets the unique identifier for the balance record.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the account ID this balance belongs to.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the type of balance (e.g., interimAvailable, expected).
    /// </summary>
    public required string BalanceType { get; set; }

    /// <summary>
    ///     Gets or sets the balance amount.
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the balance.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    ///     Gets or sets the reference date for this balance.
    /// </summary>
    public DateTime? ReferenceDate { get; set; }

    /// <summary>
    ///     Gets or sets when this balance snapshot was retrieved.
    /// </summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}
