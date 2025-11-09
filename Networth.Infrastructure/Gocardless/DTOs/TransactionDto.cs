using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Transaction schema according to GoCardless API specification.
/// </summary>
public record TransactionDto
{
    /// <summary>
    ///     Gets the transaction ID.
    /// </summary>
    [JsonPropertyName("transactionId")]
    public string? TransactionId { get; init; }

    /// <summary>
    ///     Gets the entry reference.
    /// </summary>
    [JsonPropertyName("entryReference")]
    public string? EntryReference { get; init; }

    /// <summary>
    ///     Gets the end to end ID.
    /// </summary>
    [JsonPropertyName("endToEndId")]
    public string? EndToEndId { get; init; }

    /// <summary>
    ///     Gets the mandate ID.
    /// </summary>
    [JsonPropertyName("mandateId")]
    public string? MandateId { get; init; }

    /// <summary>
    ///     Gets the check ID.
    /// </summary>
    [JsonPropertyName("checkId")]
    public string? CheckId { get; init; }

    /// <summary>
    ///     Gets the creditor ID.
    /// </summary>
    [JsonPropertyName("creditorId")]
    public string? CreditorId { get; init; }

    /// <summary>
    ///     Gets the booking date.
    /// </summary>
    [JsonPropertyName("bookingDate")]
    public string? BookingDate { get; init; }

    /// <summary>
    ///     Gets the value date.
    /// </summary>
    [JsonPropertyName("valueDate")]
    public string? ValueDate { get; init; }

    /// <summary>
    ///     Gets the booking date time.
    /// </summary>
    [JsonPropertyName("bookingDateTime")]
    public string? BookingDateTime { get; init; }

    /// <summary>
    ///     Gets the value date time.
    /// </summary>
    [JsonPropertyName("valueDateTime")]
    public string? ValueDateTime { get; init; }

    /// <summary>
    ///     Gets the transaction amount.
    /// </summary>
    [JsonPropertyName("transactionAmount")]
    [Required]
    public required TransactionAmountDto TransactionAmount { get; init; }

    /// <summary>
    ///     Gets the currency exchange information.
    /// </summary>
    [JsonPropertyName("currencyExchange")]
    public CurrencyExchangeDto[]? CurrencyExchange { get; init; }

    /// <summary>
    ///     Gets the creditor name.
    /// </summary>
    [JsonPropertyName("creditorName")]
    public string? CreditorName { get; init; }

    /// <summary>
    ///     Gets the creditor account.
    /// </summary>
    [JsonPropertyName("creditorAccount")]
    public TransactionAccountDto? CreditorAccount { get; init; }

    /// <summary>
    ///     Gets the ultimate creditor.
    /// </summary>
    [JsonPropertyName("ultimateCreditor")]
    public string? UltimateCreditor { get; init; }

    /// <summary>
    ///     Gets the debtor name.
    /// </summary>
    [JsonPropertyName("debtorName")]
    public string? DebtorName { get; init; }

    /// <summary>
    ///     Gets the debtor account.
    /// </summary>
    [JsonPropertyName("debtorAccount")]
    public TransactionAccountDto? DebtorAccount { get; init; }

    /// <summary>
    ///     Gets the ultimate debtor.
    /// </summary>
    [JsonPropertyName("ultimateDebtor")]
    public string? UltimateDebtor { get; init; }

    /// <summary>
    ///     Gets the remittance information unstructured.
    /// </summary>
    [JsonPropertyName("remittanceInformationUnstructured")]
    public string? RemittanceInformationUnstructured { get; init; }

    /// <summary>
    ///     Gets the remittance information unstructured array.
    /// </summary>
    [JsonPropertyName("remittanceInformationUnstructuredArray")]
    public string[]? RemittanceInformationUnstructuredArray { get; init; }

    /// <summary>
    ///     Gets the remittance information structured.
    /// </summary>
    [JsonPropertyName("remittanceInformationStructured")]
    public string? RemittanceInformationStructured { get; init; }

    /// <summary>
    ///     Gets the remittance information structured array.
    /// </summary>
    [JsonPropertyName("remittanceInformationStructuredArray")]
    public string[]? RemittanceInformationStructuredArray { get; init; }

    /// <summary>
    ///     Gets the additional information.
    /// </summary>
    [JsonPropertyName("additionalInformation")]
    public string? AdditionalInformation { get; init; }

    /// <summary>
    ///     Gets the purpose code.
    /// </summary>
    [JsonPropertyName("purposeCode")]
    public string? PurposeCode { get; init; }

    /// <summary>
    ///     Gets the bank transaction code.
    /// </summary>
    [JsonPropertyName("bankTransactionCode")]
    public string? BankTransactionCode { get; init; }

    /// <summary>
    ///     Gets the proprietary bank transaction code.
    /// </summary>
    [JsonPropertyName("proprietaryBankTransactionCode")]
    public string? ProprietaryBankTransactionCode { get; init; }

    /// <summary>
    ///     Gets the internal transaction ID.
    /// </summary>
    [JsonPropertyName("internalTransactionId")]
    public string? InternalTransactionId { get; init; }

    /// <summary>
    ///     Gets the balance after transaction.
    /// </summary>
    [JsonPropertyName("balanceAfterTransaction")]
    public BalanceAfterTransactionDto? BalanceAfterTransaction { get; init; }
}
