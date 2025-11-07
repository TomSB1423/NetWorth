using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Account balance amount schema according to GoCardless API specification.
/// </summary>
public record BalanceAmountDto
{
    /// <summary>
    ///     Gets the balance amount.
    /// </summary>
    [JsonPropertyName("amount")]
    [Required]
    public required string Amount { get; init; }

    /// <summary>
    ///     Gets the currency of the balance.
    /// </summary>
    [JsonPropertyName("currency")]
    [Required]
    public required string Currency { get; init; }
}
