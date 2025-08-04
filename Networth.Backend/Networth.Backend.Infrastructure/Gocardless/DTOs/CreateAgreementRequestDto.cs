using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Networth.Backend.Infrastructure.Gocardless.Enums;

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
    [Required]
    [JsonPropertyName("max_historical_days")]
    public required int MaxHistoricalDays { get; init; }

    /// <summary>
    ///     Gets maximum number of days the access token is valid.
    /// </summary>
    [Required]
    [JsonPropertyName("access_valid_for_days")]
    public required int AccessValidForDays { get; init; }

    /// <summary>
    ///     Gets list of access scopes requested for the agreement.
    /// </summary>
    [Required]
    [JsonPropertyName("access_scope")]
    public required AccessScope[] AccessScope { get; init; }
}
