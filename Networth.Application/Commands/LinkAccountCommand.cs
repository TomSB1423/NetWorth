using Networth.Application.Interfaces;

namespace Networth.Application.Commands;

/// <summary>
///     Command for linking a bank account by creating an agreement and requisition.
/// </summary>
public class LinkAccountCommand : IRequest<LinkAccountCommandResult>
{
    /// <summary>
    ///     Gets or sets the user ID who is linking the account.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the institution ID to create the agreement for.
    /// </summary>
    public required string InstitutionId { get; set; }
}
