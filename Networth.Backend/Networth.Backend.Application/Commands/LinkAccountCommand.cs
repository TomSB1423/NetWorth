namespace Networth.Backend.Application.Commands;

/// <summary>
///     Command for linking a bank account by creating an agreement and requisition.
/// </summary>
public class LinkAccountCommand
{
    /// <summary>
    ///     Gets or sets the institution ID to create the agreement for.
    /// </summary>
    public required string InstitutionId { get; set; }
}
