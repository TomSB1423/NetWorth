namespace Networth.Functions.Models.Responses;

/// <summary>
///     Represents a single net worth data point.
/// </summary>
public class NetWorthPointResponse
{
    /// <summary>
    ///     Gets the date of the data point.
    /// </summary>
    public required DateTime Date { get; init; }

    /// <summary>
    ///     Gets the total net worth amount on that date.
    /// </summary>
    public required decimal Amount { get; init; }
}
