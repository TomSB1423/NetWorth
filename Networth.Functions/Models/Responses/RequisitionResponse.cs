namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for a requisition.
/// </summary>
public record RequisitionResponse
{
    /// <summary>
    ///     Gets the unique identifier for the requisition.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the status of the requisition.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    ///     Gets the institution ID.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the agreement ID.
    /// </summary>
    public required string AgreementId { get; init; }

    /// <summary>
    ///     Gets the reference identifier.
    /// </summary>
    public required string Reference { get; init; }

    /// <summary>
    ///     Gets the accounts associated with this requisition.
    /// </summary>
    public string[] Accounts { get; init; } = [];

    /// <summary>
    ///     Gets the authorization link for the user to complete bank authentication.
    /// </summary>
    public required string AuthenticationLink { get; init; }
}
