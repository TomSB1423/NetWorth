namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for a user's bank account.
/// </summary>
public record UserAccountResponse
{
    /// <summary>
    ///     Gets the unique identifier for the account.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the institution ID.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the name of the account.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the institution GoCardless ID.
    /// </summary>
    public string? InstitutionGoCardlessId { get; init; }
}
