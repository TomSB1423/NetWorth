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
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the account ID.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the GoCardless transaction ID.
    /// </summary>
    public required string TransactionId { get; set; }

    /// <summary>
    ///     Gets or sets the debtor name.
    /// </summary>
    public string? DebtorName { get; set; }

    /// <summary>
    ///     Gets or sets the debtor account IBAN.
    /// </summary>
    public string? DebtorAccountIban { get; set; }

    /// <summary>
    ///     Gets or sets the transaction amount.
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the transaction.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    ///     Gets or sets the bank transaction code.
    /// </summary>
    public string? BankTransactionCode { get; set; }

    /// <summary>
    ///     Gets or sets the booking date.
    /// </summary>
    public DateTime? BookingDate { get; set; }

    /// <summary>
    ///     Gets or sets the value date.
    /// </summary>
    public DateTime? ValueDate { get; set; }

    /// <summary>
    ///     Gets or sets the remittance information (unstructured).
    /// </summary>
    public string? RemittanceInformationUnstructured { get; set; }

    /// <summary>
    ///     Gets or sets the transaction status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    ///     Gets or sets when this transaction was imported.
    /// </summary>
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
}
