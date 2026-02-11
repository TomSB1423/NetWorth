using System.Text.Json.Serialization;

namespace Networth.Domain.Enums;

/// <summary>
///     Represents the user-specified category for account categorization purposes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountCategory
{
    /// <summary>
    ///     An account used for everyday spending and transactions.
    /// </summary>
    Spending = 0,

    /// <summary>
    ///     An account used for saving money.
    /// </summary>
    Savings,

    /// <summary>
    ///     An account used for investments such as stocks, bonds, or funds.
    /// </summary>
    Investment,
}
