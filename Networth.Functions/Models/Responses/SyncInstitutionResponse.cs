namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response for institution sync operation.
/// </summary>
public class SyncInstitutionResponse
{
    /// <summary>
    ///     Gets or sets the institution ID that was synced.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the number of accounts enqueued for sync.
    /// </summary>
    public required int AccountsEnqueued { get; set; }

    /// <summary>
    ///     Gets or sets the list of account IDs that were enqueued.
    /// </summary>
    public required List<string> AccountIds { get; set; }
}
