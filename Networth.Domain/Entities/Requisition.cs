using Networth.Domain.Enums;

namespace Networth.Domain.Entities;

/// <summary>
///     Represents a bank account requisition for connecting to financial institutions.
/// </summary>
public class Requisition
{
    /// <summary>
    ///     Gets or sets the unique identifier for the requisition.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the creation date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    ///     Gets or sets the redirect URL after completion.
    /// </summary>
    public required string? Redirect { get; set; }

    /// <summary>
    ///     Gets or sets the status of the requisition.
    /// </summary>
    public required AccountLinkStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the institution ID.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the agreement ID.
    /// </summary>
    public required string AgreementId { get; set; }

    /// <summary>
    ///     Gets or sets the reference identifier.
    /// </summary>
    public required string Reference { get; set; }

    /// <summary>
    ///     Gets or sets the accounts associated with this requisition.
    /// </summary>
    public string[] Accounts { get; set; } = [];

    /// <summary>
    ///     Gets or sets the authorization link for the user to complete bank authentication.
    /// </summary>
    public required string AuthenticationLink { get; set; }

    /// <summary>
    ///     Gets or sets the account selection option.
    /// </summary>
    public string? AccountSelection { get; set; }
}
