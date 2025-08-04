using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.Enums;

/// <summary>
///     Represents the access scopes available for account linking with Gocardless.
/// </summary>
public enum AccessScope
{
    /// <summary>
    ///     Allows access to account balances.
    /// </summary>
    [JsonPropertyName("balances")]
    Balances,

    /// <summary>
    ///     Allows access to account transactions.
    /// </summary>
    [JsonPropertyName("transactions")]
    Transactions,

    /// <summary>
    ///     Allows access to account details.
    /// </summary>
    [JsonPropertyName("account_details")]
    AccountDetails,
}
