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
    [JsonPropertyName("agreement")]
    public string? Agreement { get; init; }

    /// <summary>
    ///     Gets the reference identifier for the requisition.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    ///     Gets the user language preference.
    /// </summary>
    [JsonPropertyName("user_language")]
    public string? UserLanguage { get; init; }

    /// <summary>
    ///     Gets the optional SSN field to verify ownership of the account.
    /// </summary>
    [JsonPropertyName("ssn")]
    public string? Ssn { get; init; }

    /// <summary>
    ///     Gets a value indicating whether gets the option to enable account selection view for the end user.
    /// </summary>
    [JsonPropertyName("account_selection")]
    public bool AccountSelection { get; init; } = false;

    /// <summary>
    ///     Gets a value indicating whether gets the option to enable redirect back to the client after account list received.
    /// </summary>
    [JsonPropertyName("redirect_immediate")]
    public bool RedirectImmediate { get; init; } = false;
}
