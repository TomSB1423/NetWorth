using Networth.Domain.Enums;

namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for linking an institution.
/// </summary>
public record LinkInstitutionResponse
{
    /// <summary>
    ///     Gets the authorization link for the user to complete bank authentication.
    ///     This will be null if the institution is already linked.
    /// </summary>
    public string? AuthorizationLink { get; init; }

    /// <summary>
    ///     Gets the status of the link.
    /// </summary>
    public required AccountLinkStatus Status { get; init; }

    /// <summary>
    ///     Gets a value indicating whether this institution is already linked.
    /// </summary>
    public bool IsAlreadyLinked { get; init; }

    /// <summary>
    ///     Gets the existing requisition ID if already linked.
    /// </summary>
    public string? ExistingRequisitionId { get; init; }
}
