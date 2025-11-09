using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Currency exchange schema according to GoCardless API specification.
/// </summary>
public record CurrencyExchangeDto
{
    /// <summary>
    ///     Gets the source currency.
    /// </summary>
    [JsonPropertyName("sourceCurrency")]
    public string? SourceCurrency { get; init; }

    /// <summary>
    ///     Gets the exchange rate.
    /// </summary>
    [JsonPropertyName("exchangeRate")]
    public string? ExchangeRate { get; init; }

    /// <summary>
    ///     Gets the unit currency.
    /// </summary>
    [JsonPropertyName("unitCurrency")]
    public string? UnitCurrency { get; init; }

    /// <summary>
    ///     Gets the target currency.
    /// </summary>
    [JsonPropertyName("targetCurrency")]
    public string? TargetCurrency { get; init; }

    /// <summary>
    ///     Gets the quotation date.
    /// </summary>
    [JsonPropertyName("quotationDate")]
    public string? QuotationDate { get; init; }

    /// <summary>
    ///     Gets the contract identification.
    /// </summary>
    [JsonPropertyName("contractIdentification")]
    public string? ContractIdentification { get; init; }
}
