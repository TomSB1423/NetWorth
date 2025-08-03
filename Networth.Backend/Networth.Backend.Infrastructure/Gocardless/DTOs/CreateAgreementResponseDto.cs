using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Response DTO for a created agreement from GoCardless according to EndUserAgreement schema.
/// </summary>
internal record CreateAgreementResponseDto
{
    /// <summary>
    ///     Gets the unique identifier for the agreement.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the creation timestamp of the agreement.
    /// </summary>
    [JsonPropertyName("created")]
    public required DateTime Created { get; init; }

    /// <summary>
    ///     Gets the institution ID this agreement is for.
    /// </summary>
    [JsonPropertyName("institution_id")]
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the maximum number of days of historical data available.
    /// </summary>
    [JsonPropertyName("max_historical_days")]
    public int MaxHistoricalDays { get; init; } = 90;

    /// <summary>
    ///     Gets the number of days the access token is valid for.
    /// </summary>
    [JsonPropertyName("access_valid_for_days")]
    public int AccessValidForDays { get; init; } = 90;

    /// <summary>
    ///     Gets the access scopes granted for this agreement.
    /// </summary>
    [JsonPropertyName("access_scope")]
    public string[] AccessScope { get; init; } = ["balances", "details", "transactions"];

    /// <summary>
    ///     Gets the acceptance timestamp of the agreement.
    /// </summary>
    [JsonPropertyName("accepted")]
    public DateTime? Accepted { get; init; }

    /// <summary>
    ///     Gets a value indicating whether gets whether this agreement can be extended.
    /// </summary>
    [JsonPropertyName("reconfirmation")]
    public bool Reconfirmation { get; init; } = false;
}
