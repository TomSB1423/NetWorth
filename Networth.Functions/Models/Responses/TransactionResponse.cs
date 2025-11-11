namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for a financial transaction.
/// </summary>
public record TransactionResponse
{
    /// <summary>
    ///     Gets the composite identifier for the transaction (accountId_transactionId).
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the account ID this transaction belongs to.
    /// </summary>
    public required string AccountId { get; init; }

    /// <summary>
    ///     Gets the transaction identifier from the bank.
    /// </summary>
    public string? TransactionId { get; init; }

    /// <summary>
    ///     Gets the transaction amount.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    ///     Gets the transaction currency code (e.g., "EUR", "USD").
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    ///     Gets the date when the transaction was booked.
    /// </summary>
    public DateTime? BookingDate { get; init; }

    /// <summary>
    ///     Gets the date when the transaction value is applied.
    /// </summary>
    public DateTime? ValueDate { get; init; }

    /// <summary>
    ///     Gets the name of the creditor (recipient).
    /// </summary>
    public string? CreditorName { get; init; }

    /// <summary>
    ///     Gets the name of the debtor (payer).
    /// </summary>
    public string? DebtorName { get; init; }

    /// <summary>
    ///     Gets the creditor's account IBAN.
    /// </summary>
    public string? CreditorAccount { get; init; }

    /// <summary>
    ///     Gets the debtor's account IBAN.
    /// </summary>
    public string? DebtorAccount { get; init; }

    /// <summary>
    ///     Gets the remittance information (transaction description).
    /// </summary>
    public string? RemittanceInformationUnstructured { get; init; }

    /// <summary>
    ///     Gets the bank transaction code.
    /// </summary>
    public string? BankTransactionCode { get; init; }

    /// <summary>
    ///     Gets the proprietary bank transaction code.
    /// </summary>
    public string? ProprietaryBankTransactionCode { get; init; }

    /// <summary>
    ///     Gets the end-to-end identification.
    /// </summary>
    public string? EndToEndId { get; init; }

    /// <summary>
    ///     Gets the mandate identifier.
    /// </summary>
    public string? MandateId { get; init; }

    /// <summary>
    ///     Gets the creditor identifier.
    /// </summary>
    public string? CreditorId { get; init; }

    /// <summary>
    ///     Gets the ultimate creditor.
    /// </summary>
    public string? UltimateCreditor { get; init; }

    /// <summary>
    ///     Gets the ultimate debtor.
    /// </summary>
    public string? UltimateDebtor { get; init; }

    /// <summary>
    ///     Gets the purpose code.
    /// </summary>
    public string? PurposeCode { get; init; }

    /// <summary>
    ///     Gets additional transaction information.
    /// </summary>
    public string? AdditionalInformation { get; init; }

    /// <summary>
    ///     Gets the account balance after this transaction.
    /// </summary>
    public decimal? BalanceAfterTransaction { get; init; }

    /// <summary>
    ///     Gets a value indicating whether this transaction is pending.
    /// </summary>
    public bool IsPending { get; init; }
}
