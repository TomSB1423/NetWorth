using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

internal record TokenRequestDto(string SecretId, string SecretKey)
{
    [JsonPropertyName("secret_id")]
    [Required]
    public required string SecretId { get; init; } = SecretId;

    [JsonPropertyName("secret_key")]
    [Required]
    public required string SecretKey { get; init; } = SecretKey;
}
