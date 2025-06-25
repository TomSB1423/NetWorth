using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.Entities;

/// <summary>
/// Represents a financial institution available for account linking.
/// </summary>
public record InstitutionDto
{
    /// <summary>
    /// Gets unique identifier for the institution.
    /// </summary>
    [Required]
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets display name of the institution.
    /// </summary>
    [Required]
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets bank Identifier Code (BIC) for the institution.
    /// </summary>
    [JsonPropertyName("bic")]
    public string? Bic { get; init; }

    /// <summary>
    /// Gets number of days of transaction history available.
    /// </summary>
    [JsonPropertyName("transaction_total_days")]
    public string TransactionTotalDays { get; init; } = "90";

    /// <summary>
    /// Gets maximum number of days the access token is valid.
    /// </summary>
    [JsonPropertyName("max_access_valid_for_days")]
    public string? MaxAccessValidForDays { get; init; }

    /// <summary>
    /// Gets list of countries where this institution operates.
    /// </summary>
    [Required]
    [JsonPropertyName("countries")]
    public required string[] Countries { get; init; }

    /// <summary>
    /// Gets uRL to the institution's logo.
    /// </summary>
    [Required]
    [JsonPropertyName("logo")]
    public required string Logo { get; init; }
}
