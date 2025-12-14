using Networth.Application.Interfaces;

namespace Networth.Application.Commands;

/// <summary>
///     Command for linking an institution by creating an agreement and requisition.
/// </summary>
public class LinkInstitutionCommand : IRequest<LinkInstitutionCommandResult>
{
    /// <summary>
    ///     Gets or sets the user ID who is linking the institution.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the institution ID to create the agreement for.
    /// </summary>
    public required string InstitutionId { get; set; }
}
