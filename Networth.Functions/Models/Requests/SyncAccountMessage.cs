namespace Networth.Functions.Models.Requests;

/// <summary>
///     Queue message model for account synchronization.
/// </summary>
public class SyncAccountMessage
{
    /// <summary>
    ///     Gets or sets the account ID to sync.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the user ID who owns the account.
    /// </summary>
    public required string UserId { get; set; }
}
