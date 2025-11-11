namespace Networth.Application.Commands;

/// <summary>
///     Result of syncing account transactions.
/// </summary>
public class SyncAccountCommandResult
{
    /// <summary>
    ///     Gets or sets the account ID that was synced.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the number of transactions synced.
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    ///     Gets or sets the date range start.
    /// </summary>
    public DateTimeOffset DateFrom { get; set; }

    /// <summary>
    ///     Gets or sets the date range end.
    /// </summary>
    public DateTimeOffset DateTo { get; set; }
}
