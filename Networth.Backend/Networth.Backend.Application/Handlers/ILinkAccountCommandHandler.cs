using Networth.Backend.Application.Commands;

namespace Networth.Backend.Application.Handlers;

/// <summary>
///     Interface for handling link account commands.
/// </summary>
public interface ILinkAccountCommandHandler
{
    /// <summary>
    ///     Handles the link account command by creating an agreement and requisition.
    /// </summary>
    /// <param name="command">The link account command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result containing both agreement and requisition details.</returns>
    Task<LinkAccountCommandResult> HandleAsync(LinkAccountCommand command, CancellationToken cancellationToken = default);
}
