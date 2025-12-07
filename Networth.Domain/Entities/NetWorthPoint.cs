namespace Networth.Domain.Entities;

/// <summary>
///     Represents a point in the net worth history.
/// </summary>
/// <param name="Date">The date of the data point.</param>
/// <param name="Amount">The total net worth amount on that date.</param>
public record NetWorthPoint(DateTime Date, decimal Amount);
