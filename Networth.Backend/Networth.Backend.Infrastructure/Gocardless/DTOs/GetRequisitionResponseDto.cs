using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

internal record GetRequisitionResponseDto
{
    /// <summary>
    ///     Gets the unique identifier for the requisition.
    /// </summary>
    [JsonPropertyName("id")]
    [Required]
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the creation timestamp of the requisition.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime? Created { get; init; }

    /// <summary>
    ///     Gets the redirect URL after completion.
    /// </summary>
    [JsonPropertyName("redirect")]
    public string? Redirect { get; init; }

    /// <summary>
    ///     Gets the status of the requisition.
    /// </summary>
    [JsonPropertyName("status")]
    [Required]
    public required string Status { get; init; }

    /// <summary>
    ///     Gets the institution ID.
    /// </summary>
    [JsonPropertyName("institution_id")]
    [Required]
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the agreement ID.
    /// </summary>
    [JsonPropertyName("agreement")]
    [Required]
    public required string Agreement { get; init; }

    /// <summary>
    ///     Gets the reference identifier.
    /// </summary>
    [JsonPropertyName("reference")]
    [Required]
    public required string Reference { get; init; }

    /// <summary>
    ///     Gets the accounts associated with this requisition.
    /// </summary>
    [JsonPropertyName("accounts")]
    public string[] Accounts { get; init; } = [];

    /// <summary>
    ///     Gets the user language.
    /// </summary>
    [JsonPropertyName("user_language")]
    [Required]
    public required string UserLanguage { get; init; }

    /// <summary>
    ///     Gets the authorization link for the user to complete bank authentication.
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; init; }

    /// <summary>
    ///     Gets a value indicating whether gets the account selection status.
    /// </summary>
    [JsonPropertyName("account_selection")]
    public bool AccountSelection { get; init; } = false;

    /// <summary>
    ///     Gets a value indicating whether gets whether the redirect is immediate.
    /// </summary>
    [JsonPropertyName("redirect_immediate")]
    public bool RedirectImmediate { get; init; } = false;

    /// <summary>
    ///     Gets the optional SSN field.
    /// </summary>
    [JsonPropertyName("ssn")]
    public string? Ssn { get; init; } = null;
}
