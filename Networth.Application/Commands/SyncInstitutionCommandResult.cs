namespace Networth.Application.Commands;

/// <summary>
///     Result of syncing an institution's accounts.
/// </summary>
public class SyncInstitutionCommandResult
{
    /// <summary>
    ///     Gets the institution ID that was synced.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the number of accounts enqueued for sync.
    /// </summary>
    public required int AccountsEnqueued { get; init; }

    /// <summary>
    ///     Gets the list of account IDs that were enqueued.
    /// </summary>
    public required IEnumerable<string> AccountIds { get; init; }
}
