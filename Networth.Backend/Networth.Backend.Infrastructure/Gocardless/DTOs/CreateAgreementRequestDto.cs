using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

internal record CreateAgreementRequestDto
{
    /// <summary>
    ///     Gets unique identifier for the institution.
    /// </summary>
    [Required]
    [JsonPropertyName("institution_id")]
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets max number of days of historical data available to retrieve.
    /// </summary>
    [JsonPropertyName("max_historical_days")]
    public int MaxHistoricalDays { get; init; } = 90;

    /// <summary>
    ///     Gets maximum number of days the access token is valid.
    /// </summary>
    [JsonPropertyName("access_valid_for_days")]
    public int AccessValidForDays { get; init; } = 90;

    /// <summary>
    ///     Gets list of access scopes requested for the agreement.
    /// </summary>
    [JsonPropertyName("access_scope")]
    public string[] AccessScope { get; init; } = ["balances", "details", "transactions"];

    /// <summary>
    ///     Gets a value indicating whether gets whether this agreement can be extended. Supported by GB banks only.
    /// </summary>
    [JsonPropertyName("reconfirmation")]
    public bool Reconfirmation { get; init; } = false;
}
