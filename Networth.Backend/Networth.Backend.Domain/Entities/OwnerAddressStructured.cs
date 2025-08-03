namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents structured owner address information.
/// </summary>
public class OwnerAddressStructured
{
    /// <summary>
    ///     Gets or sets the street name.
    /// </summary>
    public string? StreetName { get; set; }

    /// <summary>
    ///     Gets or sets the building number.
    /// </summary>
    public string? BuildingNumber { get; set; }

    /// <summary>
    ///     Gets or sets the town name.
    /// </summary>
    public string? TownName { get; set; }

    /// <summary>
    ///     Gets or sets the post code.
    /// </summary>
    public string? PostCode { get; set; }

    /// <summary>
    ///     Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }
}
