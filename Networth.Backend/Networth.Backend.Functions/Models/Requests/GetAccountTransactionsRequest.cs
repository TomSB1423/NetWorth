namespace Networth.Backend.Functions.Models.Requests;

/// <summary>
///     Request model for getting account transactions with optional date filtering.
/// </summary>
public class GetAccountTransactionsRequest
{
    /// <summary>
    ///     Gets or sets the start date for transaction filtering (optional).
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    ///     Gets or sets the end date for transaction filtering (optional).
    /// </summary>
    public DateTime? DateTo { get; set; }
}
