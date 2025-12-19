using System.Text.Json.Serialization;

namespace Networth.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccessScope
{
    /// <summary>
    ///     Access to account balances.
    /// </summary>
    Balances,

    /// <summary>
    ///     Access to account details such as account number, sort code, etc.
    /// </summary>
    Details,

    /// <summary>
    ///     Access to transaction history.
    /// </summary>
    Transactions,
}
