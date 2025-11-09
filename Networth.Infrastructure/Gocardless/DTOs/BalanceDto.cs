using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Balance schema according to GoCardless API specification.
/// </summary>
public record BalanceDto
{
    /// <summary>
    ///     Gets the balance amount and currency.
    /// </summary>
    [JsonPropertyName("balanceAmount")]
    [Required]
    public required BalanceAmountDto BalanceAmount { get; init; }

    /// <summary>
    ///     Gets the type of balance.
    /// </summary>
    [JsonPropertyName("balanceType")]
    [Required]
    public required string BalanceType { get; init; }

    /// <summary>
    ///     Gets a value indicating whether credit limit is included.
    /// </summary>
    [JsonPropertyName("creditLimitIncluded")]
    public bool? CreditLimitIncluded { get; init; }

    /// <summary>
    ///     Gets the last change date time.
    /// </summary>
    [JsonPropertyName("lastChangeDateTime")]
    public string? LastChangeDateTime { get; init; }

    /// <summary>
    ///     Gets the reference date.
    /// </summary>
    [JsonPropertyName("referenceDate")]
    public string? ReferenceDate { get; init; }

    /// <summary>
    ///     Gets the last committed transaction.
    /// </summary>
    [JsonPropertyName("lastCommittedTransaction")]
    public string? LastCommittedTransaction { get; init; }
}
