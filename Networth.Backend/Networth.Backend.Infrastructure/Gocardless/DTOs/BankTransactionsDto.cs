using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Bank transactions container according to GoCardless API specification.
/// </summary>
public record BankTransactionsDto
{
    /// <summary>
    ///     Gets the booked transactions.
    /// </summary>
    [JsonPropertyName("booked")]
    public required TransactionDto[] Booked { get; init; }

    /// <summary>
    ///     Gets the pending transactions.
    /// </summary>
    [JsonPropertyName("pending")]
    public TransactionDto[]? Pending { get; init; }
}
