using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Account balance response DTO according to GoCardless API specification.
/// </summary>
public record GetAccountBalanceResponseDto
{
    /// <summary>
    ///     Gets the array of balance information.
    /// </summary>
    [JsonPropertyName("balances")]
    [Required]
    public required BalanceDto[] Balances { get; init; }
}
