namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for account metadata.
/// </summary>
public record AccountResponse
{
    /// <summary>
    ///     Gets the unique identifier for the account.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the institution identifier this account belongs to.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the status of the account.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    ///     Gets the name of the account.
    /// </summary>
    public string? Name { get; init; }
}
