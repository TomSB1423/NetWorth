namespace Networth.Domain.Entities;

/// <summary>
///     Represents a user's bank account stored in the database.
/// </summary>
public class UserAccount
{
    /// <summary>
    ///     Gets the unique identifier for the account.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the owner user ID.
    /// </summary>
    public required string OwnerId { get; init; }

    /// <summary>
    ///     Gets the institution ID.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the name of the account.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the institution GoCardless ID.
    /// </summary>
    public string? InstitutionGoCardlessId { get; init; }
}
