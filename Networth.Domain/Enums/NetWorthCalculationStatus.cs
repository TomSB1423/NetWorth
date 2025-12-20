using System.Text.Json.Serialization;

namespace Networth.Domain.Enums;

/// <summary>
///     Represents the calculation status of the net worth history.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NetWorthCalculationStatus
{
    /// <summary>
    ///     No net worth data has been calculated yet.
    /// </summary>
    NotCalculated = 0,

    /// <summary>
    ///     Net worth calculation is currently in progress.
    /// </summary>
    Calculating,

    /// <summary>
    ///     Net worth has been calculated and data is available.
    /// </summary>
    Calculated,
}
