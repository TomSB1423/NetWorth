using Networth.Domain.Enums;

namespace Networth.Domain.Entities;

/// <summary>
///     Represents an agreement for accessing bank account data through GoCardless.
/// </summary>
public class Agreement
{
    /// <summary>
    ///     Gets or sets the unique identifier for the agreement.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the creation timestamp of the agreement.
    /// </summary>
    public required DateTime Created { get; set; }

    /// <summary>
    ///     Gets or sets the institution ID this agreement is for.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of days of historical data available.
    /// </summary>
    public int? MaxHistoricalDays { get; set; }

    /// <summary>
    ///     Gets or sets the number of days the access token is valid for.
    /// </summary>
    public int? AccessValidForDays { get; set; }

    /// <summary>
    ///     Gets or sets the access scopes granted for this agreement.
    /// </summary>
    public AccessScope[] AccessScope { get; set; } = [];

    /// <summary>
    ///     Gets or sets the acceptance timestamp of the agreement.
    /// </summary>
    public DateTime? Accepted { get; set; }
}
