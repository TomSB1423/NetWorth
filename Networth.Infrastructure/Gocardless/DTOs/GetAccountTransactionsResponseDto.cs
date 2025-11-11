using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

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
}
