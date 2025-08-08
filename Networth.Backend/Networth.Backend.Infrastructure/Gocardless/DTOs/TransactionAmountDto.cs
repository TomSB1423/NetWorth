using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Transaction amount schema according to GoCardless API specification.
/// </summary>
public record TransactionAmountDto
{
    /// <summary>
    ///     Gets the transaction amount.
    /// </summary>
    [JsonPropertyName("amount")]
    [Required]
    public required string Amount { get; init; }

    /// <summary>
    ///     Gets the currency of the transaction.
    /// </summary>
    [JsonPropertyName("currency")]
    [Required]
    public required string Currency { get; init; }
}
