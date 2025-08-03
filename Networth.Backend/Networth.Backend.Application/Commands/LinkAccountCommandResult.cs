using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Application.Commands;

/// <summary>
///     Result of the link account command containing both agreement and requisition details.
/// </summary>
public class LinkAccountCommandResult
{
    /// <summary>
    ///     Gets or sets the created agreement.
    /// </summary>
    public required Agreement Agreement { get; set; }

    /// <summary>
    ///     Gets or sets the created requisition.
    /// </summary>
    public required Requisition Requisition { get; set; }
}
