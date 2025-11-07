using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Account detail response DTO according to GoCardless API specification.
/// </summary>
public record GetAccountDetailResponseDto
{
    /// <summary>
    ///     Gets the account details.
    /// </summary>
    [JsonPropertyName("account")]
    [Required]
    public required AccountDetailSchemaDto Account { get; init; }
}
