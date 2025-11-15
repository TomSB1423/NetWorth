namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a user of the Networth application.
/// </summary>
public class User
{
    /// <summary>
    ///     Gets or sets the unique identifier for the user.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the accounts owned by this user.
    /// </summary>
    public ICollection<Account> Accounts { get; set; } = [];

    /// <summary>
    ///     Gets or sets the transactions owned by this user.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];
}
