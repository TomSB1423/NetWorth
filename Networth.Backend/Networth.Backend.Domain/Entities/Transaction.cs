namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents a transaction within a bank account according to GoCardless API.
/// </summary>
public class Transaction
{
    /// <summary>
    ///     Gets or sets the unique identifier for the transaction (primary key).
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the account ID this transaction belongs to.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the transaction ID from the bank.
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    ///     Gets or sets the transaction amount.
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the transaction.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    ///     Gets or sets the booking date.
    ///     The date when the payment is settled (when funds are available to the account owner).
    /// </summary>
    public DateTime? BookingDate { get; set; }

    /// <summary>
    ///     Gets or sets the value date.
    ///     The date when the payment is initiated (when funds cease to be available to the account owner).
    /// </summary>
    public DateTime? ValueDate { get; set; }

    /// <summary>
    ///     Gets or sets the creditor name.
    /// </summary>
    public string? CreditorName { get; set; }

    /// <summary>
    ///     Gets or sets the debtor name.
    /// </summary>
    public string? DebtorName { get; set; }

    /// <summary>
    ///     Gets or sets the debtor account IBAN.
    /// </summary>
    public string? DebtorAccount { get; set; }

    /// <summary>
    ///     Gets or sets the creditor account IBAN.
    /// </summary>
    public string? CreditorAccount { get; set; }

    /// <summary>
    ///     Gets or sets the remittance information unstructured (description).
    /// </summary>
    public string? RemittanceInformationUnstructured { get; set; }

    /// <summary>
    ///     Gets or sets the bank transaction code.
    /// </summary>
    public string? BankTransactionCode { get; set; }

    /// <summary>
    ///     Gets or sets the proprietary bank transaction code.
    /// </summary>
    public string? ProprietaryBankTransactionCode { get; set; }

    /// <summary>
    ///     Gets or sets the end to end ID.
    /// </summary>
    public string? EndToEndId { get; set; }

    /// <summary>
    ///     Gets or sets the mandate ID.
    /// </summary>
    public string? MandateId { get; set; }

    /// <summary>
    ///     Gets or sets the creditor ID.
    /// </summary>
    public string? CreditorId { get; set; }

    /// <summary>
    ///     Gets or sets the ultimate creditor.
    /// </summary>
    public string? UltimateCreditor { get; set; }

    /// <summary>
    ///     Gets or sets the ultimate debtor.
    /// </summary>
    public string? UltimateDebtor { get; set; }

    /// <summary>
    ///     Gets or sets the purpose code.
    /// </summary>
    public string? PurposeCode { get; set; }

    /// <summary>
    ///     Gets or sets additional information.
    /// </summary>
    public string? AdditionalInformation { get; set; }

    /// <summary>
    ///     Gets or sets the balance after this transaction.
    /// </summary>
    public decimal? BalanceAfterTransaction { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this is a pending transaction.
    /// </summary>
    public bool IsPending { get; set; }

    /// <summary>
    ///     Gets or sets the category of the transaction (for internal use).
    /// </summary>
    public string? Category { get; set; }
}
