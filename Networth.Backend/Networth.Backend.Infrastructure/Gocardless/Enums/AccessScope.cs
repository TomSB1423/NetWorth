namespace Networth.Backend.Infrastructure.Gocardless.Enums;

/// <summary>
///     Represents the access scopes available for account linking with Gocardless.
/// </summary>
public enum AccessScope
{
    /// <summary>
    ///     Allows access to account balances.
    /// </summary>
    Balances,

    /// <summary>
    ///     Allows access to account transactions.
    /// </summary>
    Transactions,

    /// <summary>
    ///     Allows access to account details.
    /// </summary>
    AccountDetails
}
