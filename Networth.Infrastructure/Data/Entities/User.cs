namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a user of the Networth application.
/// </summary>
public class User
{
    /// <summary>
    ///     Gets or sets the internal unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase UID from the identity provider.
    ///     This is the external identifier used for authentication.
    /// </summary>
    public required string FirebaseUid { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or sets the accounts owned by this user.
    /// </summary>
    public ICollection<Account> Accounts { get; set; } = [];

    /// <summary>
    ///     Gets or sets the transactions owned by this user.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];
}
