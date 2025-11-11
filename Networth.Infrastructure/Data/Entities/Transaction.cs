namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a financial transaction.
/// </summary>
public class Transaction
{
    /// <summary>
    ///     Gets or sets the unique identifier for the transaction.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the owner user ID.
    /// </summary>
    public required string OwnerId { get; set; }

    /// <summary>
    ///     Gets or sets the owner user.
    /// </summary>
    public User Owner { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the account ID.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the account.
    /// </summary>
    public Account Account { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the transaction value/amount.
    /// </summary>
    public required decimal Value { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the transaction.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    ///     Gets or sets the transaction time/date.
    /// </summary>
    public required DateTime Time { get; set; }
}
