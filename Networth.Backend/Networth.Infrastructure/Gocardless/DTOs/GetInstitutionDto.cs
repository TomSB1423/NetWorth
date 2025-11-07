using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Represents a financial institution available for account linking according to GoCardless API specification.
/// </summary>
public record GetInstitutionDto
{
    /// <summary>
    ///     Gets unique identifier for the institution.
    /// </summary>
    [JsonPropertyName("id")]
    [Required]
    public required string Id { get; init; }

    /// <summary>
    ///     Gets display name of the institution.
    /// </summary>
    [JsonPropertyName("name")]
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     Gets bank Identifier Code (BIC) for the institution.
    /// </summary>
    [JsonPropertyName("bic")]
    public string? Bic { get; init; }

    /// <summary>
    ///     Gets number of days of transaction history available.
    /// </summary>
    [JsonPropertyName("transaction_total_days")]
    public string? TransactionTotalDays { get; init; }

    /// <summary>
    ///     Gets maximum number of days the access token is valid.
    /// </summary>
    [JsonPropertyName("max_access_valid_for_days")]
    public string? MaxAccessValidForDays { get; init; }

    /// <summary>
    ///     Gets the maximum number of days the access token is valid before reconfirmation is required.
    /// </summary>
    [JsonPropertyName("max_access_valid_for_days_reconfirmation")]
    public string? MaxAccessValidForDaysReconfirmation { get; init; }

    /// <summary>
    ///     Gets list of countries where this institution operates.
    /// </summary>
    [JsonPropertyName("countries")]
    [Required]
    public required string[] Countries { get; init; }

    /// <summary>
    ///     Gets URL to the institution's logo.
    /// </summary>
    [JsonPropertyName("logo")]
    [Required]
    public required string Logo { get; init; }

    /// <summary>
    ///     Gets list of supported features by the institution.
    /// </summary>
    [JsonPropertyName("supported_features")]
    public string[] SupportedFeatures { get; init; } = [];

    /// <summary>
    ///     Gets identification codes for the institution.
    /// </summary>
    [JsonPropertyName("identification_codes")]
    public string[] IdentificationCodes { get; init; } = [];
}


