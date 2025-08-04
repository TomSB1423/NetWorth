using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Account detail schema according to GoCardless API specification.
/// </summary>
public record AccountDetailSchemaDto
{
    /// <summary>
    ///     Gets the resource ID.
    /// </summary>
    [JsonPropertyName("resourceId")]
    public string? ResourceId { get; init; }

    /// <summary>
    ///     Gets the IBAN.
    /// </summary>
    [JsonPropertyName("iban")]
    public string? Iban { get; init; }

    /// <summary>
    ///     Gets the BBAN.
    /// </summary>
    [JsonPropertyName("bban")]
    public string? Bban { get; init; }

    /// <summary>
    ///     Gets the SortCodeAccountNumber returned by some UK banks.
    /// </summary>
    [JsonPropertyName("scan")]
    public string? Scan { get; init; }

    /// <summary>
    ///     Gets the MSISDN.
    /// </summary>
    [JsonPropertyName("msisdn")]
    public string? Msisdn { get; init; }

    /// <summary>
    ///     Gets the currency.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    ///     Gets the owner name.
    /// </summary>
    [JsonPropertyName("ownerName")]
    public string? OwnerName { get; init; }

    /// <summary>
    ///     Gets the account name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Gets the display name.
    /// </summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    /// <summary>
    ///     Gets the product.
    /// </summary>
    [JsonPropertyName("product")]
    public string? Product { get; init; }

    /// <summary>
    ///     Gets the cash account type.
    /// </summary>
    [JsonPropertyName("cashAccountType")]
    public string? CashAccountType { get; init; }

    /// <summary>
    ///     Gets the status.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    ///     Gets the BIC.
    /// </summary>
    [JsonPropertyName("bic")]
    public string? Bic { get; init; }

    /// <summary>
    ///     Gets the linked accounts.
    /// </summary>
    [JsonPropertyName("linkedAccounts")]
    public string? LinkedAccounts { get; init; }

    /// <summary>
    ///     Gets the masked PAN.
    /// </summary>
    [JsonPropertyName("maskedPan")]
    public string? MaskedPan { get; init; }

    /// <summary>
    ///     Gets the usage.
    /// </summary>
    [JsonPropertyName("usage")]
    public string? Usage { get; init; }

    /// <summary>
    ///     Gets the details.
    /// </summary>
    [JsonPropertyName("details")]
    public string? Details { get; init; }

    /// <summary>
    ///     Gets the owner address unstructured.
    /// </summary>
    [JsonPropertyName("ownerAddressUnstructured")]
    public string[]? OwnerAddressUnstructured { get; init; }

    /// <summary>
    ///     Gets the owner address structured.
    /// </summary>
    [JsonPropertyName("ownerAddressStructured")]
    public OwnerAddressStructuredDto? OwnerAddressStructured { get; init; }

    /// <summary>
    ///     Gets additional account data used for information outside of Berlin Group specification.
    /// </summary>
    [JsonPropertyName("additionalAccountData")]
    public AdditionalAccountDataDto? AdditionalAccountData { get; init; }
}
