using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Account schema for transactions according to GoCardless API specification.
/// </summary>
public record TransactionAccountDto
{
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
    ///     Gets the PAN.
    /// </summary>
    [JsonPropertyName("pan")]
    public string? Pan { get; init; }

    /// <summary>
    ///     Gets the masked PAN.
    /// </summary>
    [JsonPropertyName("maskedPan")]
    public string? MaskedPan { get; init; }

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
}
