using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

internal record TokenRequestDto(string SecretId, string SecretKey)
{
    [JsonPropertyName("secret_id")]
    public required string SecretId { get; init; } = SecretId;

    [JsonPropertyName("secret_key")]
    public required string SecretKey { get; init; } = SecretKey;
}
