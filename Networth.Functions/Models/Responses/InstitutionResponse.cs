namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for a financial institution.
/// </summary>
/// <param name="Id">The unique identifier for the institution.</param>
/// <param name="Name">The name of the institution.</param>
/// <param name="LogoUrl">The logo URL of the institution.</param>
public record InstitutionResponse(string Id, string Name, string? LogoUrl);
