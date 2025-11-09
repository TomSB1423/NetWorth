namespace Networth.Domain.Entities;

/// <summary>
///     Represents account balance information.
/// </summary>
public class AccountBalance
{
    /// <summary>
    ///     Gets or sets the balance amount.
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    ///     Gets or sets the currency of the balance.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    ///     Gets or sets the type of balance.
    /// </summary>
    public required string BalanceType { get; set; }

    /// <summary>
    ///     Gets or sets whether credit limit is included.
    /// </summary>
    public bool? CreditLimitIncluded { get; set; }

    /// <summary>
    ///     Gets or sets the reference date.
    /// </summary>
    public DateTime? ReferenceDate { get; set; }
}
