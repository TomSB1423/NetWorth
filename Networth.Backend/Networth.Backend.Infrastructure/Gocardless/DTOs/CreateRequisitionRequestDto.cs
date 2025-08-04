using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Request DTO for creating a requisition with GoCardless according to RequisitionRequest schema.
/// </summary>
internal record CreateRequisitionRequestDto
{
    /// <summary>
    ///     Gets the redirect URL after the user completes the bank authentication.
    /// </summary>
    [Required]
    [JsonPropertyName("redirect")]
    public required string Redirect { get; init; }

    /// <summary>
    ///     Gets the institution ID for the bank.
    /// </summary>
    [Required]
    [JsonPropertyName("institution_id")]
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the agreement ID associated with this requisition.
    /// </summary>
    [Required]
    [JsonPropertyName("agreement")]
    public required string Agreement { get; init; }

    /// <summary>
    ///     Gets the reference identifier for the requisition.
    /// </summary>
    [Required]
    [JsonPropertyName("reference")]
    public required string Reference { get; init; }

    /// <summary>
    ///     Gets the user language preference.
    /// </summary>
    [Required]
    [JsonPropertyName("user_language")]
    public required string UserLanguage { get; init; }
}
