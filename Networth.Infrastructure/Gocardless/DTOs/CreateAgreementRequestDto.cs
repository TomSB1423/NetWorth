using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Networth.Domain.Enums;

namespace Networth.Infrastructure.Gocardless.DTOs;

internal record CreateAgreementRequestDto
{
    /// <summary>
    ///     Gets unique identifier for the institution.
    /// </summary>
    [JsonPropertyName("institution_id")]
    [Required]
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets max number of days of historical data available to retrieve.
    /// </summary>
    [JsonPropertyName("max_historical_days")]
    public int? MaxHistoricalDays { get; init; }

    /// <summary>
    ///     Gets maximum number of days the access token is valid.
    /// </summary>
    [JsonPropertyName("access_valid_for_days")]
    public int? AccessValidForDays { get; init; }

    /// <summary>
    ///     Gets list of access scopes requested for the agreement.
    /// </summary>
    [JsonPropertyName("access_scope")]
    [Required]
    public required AccessScope[] AccessScope { get; init; }
}
