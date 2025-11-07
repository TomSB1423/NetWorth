using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Owner address structured schema according to GoCardless API specification.
/// </summary>
public record OwnerAddressStructuredDto
{
    /// <summary>
    ///     Gets the street name.
    /// </summary>
    [JsonPropertyName("streetName")]
    public string? StreetName { get; init; }

    /// <summary>
    ///     Gets the building number.
    /// </summary>
    [JsonPropertyName("buildingNumber")]
    public string? BuildingNumber { get; init; }

    /// <summary>
    ///     Gets the town name.
    /// </summary>
    [JsonPropertyName("townName")]
    public string? TownName { get; init; }

    /// <summary>
    ///     Gets the post code.
    /// </summary>
    [JsonPropertyName("postCode")]
    public string? PostCode { get; init; }

    /// <summary>
    ///     Gets the country.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; init; }
}
