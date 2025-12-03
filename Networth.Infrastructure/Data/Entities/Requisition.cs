using Networth.Domain.Enums;

namespace Networth.Infrastructure.Data.Entities;

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
    ///     Gets or sets the user ID who owns this requisition.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the redirect URL after completion.
    /// </summary>
    public string? Redirect { get; set; }

    /// <summary>
    ///     Gets or sets the status of the requisition.
    /// </summary>
    public required AccountLinkStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the institution metadata ID.
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
    ///     Gets or sets the accounts associated with this requisition (JSON array of account IDs).
    /// </summary>
    public string Accounts { get; set; } = "[]";

    /// <summary>
    ///     Gets or sets the user language.
    /// </summary>
    public string? UserLanguage { get; set; }

    /// <summary>
    ///     Gets or sets the authorization link for the user to complete bank authentication.
    /// </summary>
    public required string Link { get; set; }

    /// <summary>
    ///     Gets or sets the SSN if required by the institution.
    /// </summary>
    public string? Ssn { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether account selection is enabled.
    /// </summary>
    public bool AccountSelection { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether immediate redirect is enabled.
    /// </summary>
    public bool RedirectImmediate { get; set; }

    /// <summary>
    ///     Gets or sets the creation date.
    /// </summary>
    public DateTime Created { get; set; }
}
