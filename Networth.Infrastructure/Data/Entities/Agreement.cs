using Networth.Domain.Enums;

namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a user's agreement for accessing bank account data through GoCardless.
/// </summary>
public class Agreement
{
    /// <summary>
    ///     Gets or sets the unique identifier for the agreement.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the user ID who owns this agreement.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the institution metadata ID this agreement is for.
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
    ///     Gets or sets the access scopes granted for this agreement (JSON array).
    /// </summary>
    public required string AccessScope { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether reconfirmation is required.
    /// </summary>
    public bool Reconfirmation { get; set; }

    /// <summary>
    ///     Gets or sets the creation timestamp of the agreement.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    ///     Gets or sets the acceptance timestamp of the agreement.
    /// </summary>
    public DateTime? Accepted { get; set; }

    /// <summary>
    ///     Gets or sets the requisitions associated with this agreement.
    /// </summary>
    public ICollection<Requisition> Requisitions { get; set; } = [];
}
