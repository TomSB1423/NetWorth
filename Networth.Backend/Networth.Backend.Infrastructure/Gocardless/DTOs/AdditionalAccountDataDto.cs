using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Additional account data schema according to GoCardless API specification.
/// </summary>
public record AdditionalAccountDataDto
{
    /// <summary>
    ///     Gets the secondary identification.
    /// </summary>
    [JsonPropertyName("secondaryIdentification")]
    public string? SecondaryIdentification { get; init; }
}
