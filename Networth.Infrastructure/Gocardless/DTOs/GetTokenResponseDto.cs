using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

internal class GetTokenResponseDto
{
    [JsonPropertyName("access")]
    [Required]
    public required string Access { get; set; }

    [JsonPropertyName("access_expires")]
    public int? AccessExpires { get; set; }
}
