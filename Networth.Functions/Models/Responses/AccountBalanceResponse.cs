namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for account balance information.
/// </summary>
public class AccountBalanceResponse
{
    /// <summary>
    ///     Gets the balance amount.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    ///     Gets the currency of the balance.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    ///     Gets the type of balance.
    /// </summary>
    public required string BalanceType { get; init; }

    /// <summary>
    ///     Gets whether credit limit is included.
    /// </summary>
    public bool? CreditLimitIncluded { get; init; }

    /// <summary>
    ///     Gets the reference date.
    /// </summary>
    public DateTime? ReferenceDate { get; init; }
}
