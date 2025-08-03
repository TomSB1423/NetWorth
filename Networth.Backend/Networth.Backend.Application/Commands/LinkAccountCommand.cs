namespace Networth.Backend.Application.Commands;

/// <summary>
///     Command for linking a bank account by creating an agreement and requisition.
/// </summary>
public class LinkAccountCommand
{
    /// <summary>
    ///     Gets or sets the institution ID to create the agreement for.
    /// </summary>
    public required string InstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the URL to redirect to after completion.
    /// </summary>
    public required string RedirectUrl { get; set; }

    /// <summary>
    ///     Gets or sets the reference identifier for the requisition.
    /// </summary>
    public required string Reference { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of days for historical data access. Default is 90.
    /// </summary>
    public int MaxHistoricalDays { get; set; } = 90;

    /// <summary>
    ///     Gets or sets the number of days the access is valid for. Default is 90.
    /// </summary>
    public int AccessValidForDays { get; set; } = 90;

    /// <summary>
    ///     Gets or sets the user language preference. Default is "EN".
    /// </summary>
    public string UserLanguage { get; set; } = "EN";
}
