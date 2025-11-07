using System.ComponentModel.DataAnnotations;

namespace Networth.Backend.Functions.Models.Requests;

/// <summary>
///     Request model for creating an agreement.
/// </summary>
public class CreateAgreementRequest
{
    /// <summary>
    ///     Gets or sets the institution ID to create the agreement for.
    /// </summary>
    [Required]
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of days for historical data access. Default is 90.
    /// </summary>
    public int MaxHistoricalDays { get; set; } = 90;

    /// <summary>
    ///     Gets or sets the number of days the access is valid for. Default is 90.
    /// </summary>
    public int AccessValidForDays { get; set; } = 90;
}
