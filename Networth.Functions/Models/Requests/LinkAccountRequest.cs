namespace Networth.Functions.Models.Requests;

/// <summary>
///     Request model for linking a bank account.
/// </summary>
public class LinkAccountRequest
{
    /// <summary>
    ///     Gets or sets the user ID who is linking the account.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the institution ID to create the agreement for.
    /// </summary>
    public required string InstitutionId { get; set; }
}
