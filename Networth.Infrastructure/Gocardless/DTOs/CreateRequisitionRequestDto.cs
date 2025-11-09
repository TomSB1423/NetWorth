using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Request DTO for creating a requisition with GoCardless according to RequisitionRequest schema.
/// </summary>
internal record CreateRequisitionRequestDto
{
    /// <summary>
    ///     Gets the redirect URL after the user completes the bank authentication.
    /// </summary>
    [JsonPropertyName("redirect")]
    [Required]
    public required string Redirect { get; init; }

    /// <summary>
    ///     Gets the institution ID for the bank.
    /// </summary>
    [JsonPropertyName("institution_id")]
    [Required]
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the optional agreement ID associated with this requisition.
    /// </summary>
    [JsonPropertyName("agreement")]
    public string? Agreement { get; init; }

    /// <summary>
    ///     Gets the optional client set unique identifier for the requisition.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    ///     Gets the optional user language preference.
    /// </summary>
    [JsonPropertyName("user_language")]
    public string? UserLanguage { get; init; }
}
