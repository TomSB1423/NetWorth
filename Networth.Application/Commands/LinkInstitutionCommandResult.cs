using Networth.Domain.Enums;

namespace Networth.Application.Commands;

/// <summary>
///     Result of the link institution command containing both agreement and requisition details.
/// </summary>
public class LinkInstitutionCommandResult
{
    /// <summary>
    ///     Gets or sets the authorization link for the user to complete bank authentication.
    ///     This will be null if the institution is already linked.
    /// </summary>
    public string? AuthorizationLink { get; set; }

    /// <summary>
    ///     Gets or sets status of the link.
    /// </summary>
    public required AccountLinkStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this institution is already linked.
    /// </summary>
    public bool IsAlreadyLinked { get; set; }

    /// <summary>
    ///     Gets or sets the existing requisition ID if already linked.
    /// </summary>
    public string? ExistingRequisitionId { get; set; }
}
