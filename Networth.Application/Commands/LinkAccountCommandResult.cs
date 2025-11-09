using Networth.Domain.Enums;

namespace Networth.Application.Commands;

/// <summary>
///     Result of the link account command containing both agreement and requisition details.
/// </summary>
public class LinkAccountCommandResult
{
    /// <summary>
    ///     Gets or sets the authorization link for the user to complete bank authentication.
    /// </summary>
    public required string AuthorizationLink { get; set; }

    /// <summary>
    ///     Gets or sets status of the link.
    /// </summary>
    public required AccountLinkStatus Status { get; set; }
}
