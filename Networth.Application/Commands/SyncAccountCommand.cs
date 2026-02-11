using Networth.Application.Interfaces;

namespace Networth.Application.Commands;

/// <summary>
///     Command to sync account transactions from GoCardless to the database.
/// </summary>
public class SyncAccountCommand : IRequest<SyncAccountCommandResult>
{
    /// <summary>
    ///     Gets or sets the account ID to synchronize.
    /// </summary>
    public required string AccountId { get; set; }

    /// <summary>
    ///     Gets or sets the internal user ID who owns the account.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the optional start date for transaction sync.
    ///     If not provided, will use default lookback period.
    /// </summary>
    public DateTimeOffset? DateFrom { get; set; }

    /// <summary>
    ///     Gets or sets the optional end date for transaction sync.
    ///     If not provided, will use current date.
    /// </summary>
    public DateTimeOffset? DateTo { get; set; }
}
