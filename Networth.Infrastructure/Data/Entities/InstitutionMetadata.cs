namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents cached metadata about a financial institution from GoCardless.
/// </summary>
public class InstitutionMetadata
{
    /// <summary>
    ///     Gets or sets the unique identifier for the institution.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of the bank.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the logo URL of the bank institution.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    ///     Gets or sets the BIC code of the institution.
    /// </summary>
    public string? Bic { get; set; }

    /// <summary>
    ///     Gets or sets the country code this institution operates in.
    /// </summary>
    public required string CountryCode { get; set; }

    /// <summary>
    ///     Gets or sets the countries where this institution operates (JSON array).
    /// </summary>
    public required string Countries { get; set; }

    /// <summary>
    ///     Gets or sets the supported features of this institution (JSON array).
    /// </summary>
    public string? SupportedFeatures { get; set; }

    /// <summary>
    ///     Gets or sets when this record was last updated.
    /// </summary>
    public required DateTime LastUpdated { get; set; }
}
