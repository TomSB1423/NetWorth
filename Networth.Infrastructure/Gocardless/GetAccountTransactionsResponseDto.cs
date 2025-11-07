using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Networth.Infrastructure.Gocardless.DTOs;

namespace Networth.Infrastructure.Gocardless;

/// <summary>
///     Account transactions response DTO according to GoCardless API specification.
/// </summary>
internal record GetAccountTransactionsResponseDto
{
    /// <summary>
    ///     Gets the transactions container with booked and pending transactions.
    /// </summary>
    [JsonPropertyName("transactions")]
    [Required]
    public required BankTransactionsDto Transactions { get; init; }

    /// <summary>
    ///     Gets the last time the account transactions were updated.
    /// </summary>
    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; init; }
}
