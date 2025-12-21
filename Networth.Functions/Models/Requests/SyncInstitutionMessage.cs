namespace Networth.Functions.Models.Requests;

/// <summary>
///     Queue message model for institution synchronization.
/// </summary>
public class SyncInstitutionMessage
{
    /// <summary>
    ///     Gets or sets the institution ID to sync.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the internal user ID who owns the accounts.
    /// </summary>
    public required Guid UserId { get; set; }
}
