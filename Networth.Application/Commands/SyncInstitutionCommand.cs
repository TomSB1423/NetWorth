using Networth.Application.Interfaces;

namespace Networth.Application.Commands;

/// <summary>
///     Command to sync all accounts belonging to an institution for a specific user.
/// </summary>
public class SyncInstitutionCommand : IRequest<SyncInstitutionCommandResult>
{
    /// <summary>
    ///     Gets or sets the institution ID to synchronize.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the user ID who owns the accounts.
    /// </summary>
    public required string UserId { get; set; }

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
