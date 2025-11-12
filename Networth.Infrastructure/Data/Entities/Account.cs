namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a bank account.
/// </summary>
public class Account
{
    /// <summary>
    ///     Gets or sets the unique identifier for the account.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the owner user ID.
    /// </summary>
    public required string OwnerId { get; set; }

    /// <summary>
    ///     Gets or sets the owner user.
    /// </summary>
    public User Owner { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the user-institution link ID.
    /// </summary>
    public required string UserInstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the user-institution link.
    /// </summary>
    public Institution Institution { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the GoCardless institution ID (denormalized for convenience).
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the account.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the transactions associated with this account.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];
}
