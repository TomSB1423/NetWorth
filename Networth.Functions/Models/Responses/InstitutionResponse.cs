namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for a financial institution.
/// </summary>
public record InstitutionResponse
{
    /// <summary>
    ///     Gets the unique identifier for the institution.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the name of the institution.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the logo URL of the institution.
    /// </summary>
    public string? LogoUrl { get; init; }
}
