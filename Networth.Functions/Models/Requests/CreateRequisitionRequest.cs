using System.ComponentModel.DataAnnotations;

namespace Networth.Backend.Functions.Models.Requests;

/// <summary>
///     Request model for creating a requisition.
/// </summary>
public class CreateRequisitionRequest
{
    /// <summary>
    ///     Gets or sets the URL to redirect to after completion.
    /// </summary>
    [Required]
    public required string RedirectUrl { get; set; }

    /// <summary>
    ///     Gets or sets the institution ID.
    /// </summary>
    [Required]
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the agreement ID.
    /// </summary>
    [Required]
    public required string AgreementId { get; set; }

    /// <summary>
    ///     Gets or sets the reference identifier for the requisition.
    /// </summary>
    [Required]
    public required string Reference { get; set; }

    /// <summary>
    ///     Gets or sets the user language preference. Default is "EN".
    /// </summary>
    public string UserLanguage { get; set; } = "EN";
}
