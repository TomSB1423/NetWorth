namespace Networth.Application.Interfaces;

/// <summary>
///     Institution information for account DTO.
/// </summary>
public class InstitutionInfo
{
    /// <summary>
    ///     Gets the institution ID.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the GoCardless institution ID.
    /// </summary>
    public required string GoCardlessId { get; init; }
}
