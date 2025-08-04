using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.Enums;

/// <summary>
///     Represents the available access scopes for an agreement.
/// </summary>
public enum AccessScope
{
    /// <summary>
    ///     Access to account details.
    /// </summary>
    Details,

    /// <summary>
    ///     Access to account balances.
    /// </summary>
    Balances,

    /// <summary>
    ///     Access to account transactions.
    /// </summary>
    Transactions,
}
