using System.Text.Json.Serialization;

namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

/// <summary>
///     Account metadata DTO according to GoCardless API specification.
/// </summary>
public record GetAccountResponseDto
{
    /// <summary>
    ///     Gets the ID of this Account, used to refer to this account in other API calls.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the date and time at which the account object was created.
    /// </summary>
    [JsonPropertyName("created")]
    public required DateTime Created { get; init; }

    /// <summary>
    ///     Gets the date and time at which the account object was last accessed.
    /// </summary>
    [JsonPropertyName("last_accessed")]
    public required DateTime LastAccessed { get; init; }

    /// <summary>
    ///     Gets the Account IBAN.
    /// </summary>
    [JsonPropertyName("iban")]
    public string? Iban { get; init; }

    /// <summary>
    ///     Gets the Account BBAN.
    /// </summary>
    [JsonPropertyName("bban")]
    public string? Bban { get; init; }

    /// <summary>
    ///     Gets the processing status of this account.
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    ///     Gets the ASPSP associated with this account.
    /// </summary>
    [JsonPropertyName("institution_id")]
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the name of the account owner.
    /// </summary>
    [JsonPropertyName("owner_name")]
    public string? OwnerName { get; init; }

    /// <summary>
    ///     Gets the name of account.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
