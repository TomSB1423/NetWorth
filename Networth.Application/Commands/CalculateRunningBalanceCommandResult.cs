namespace Networth.Application.Commands;

/// <summary>
///     Result of the CalculateRunningBalanceCommand.
/// </summary>
public class CalculateRunningBalanceCommandResult
{
    /// <summary>
    ///     Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     Gets or sets the number of transactions processed.
    /// </summary>
    public int ProcessedTransactions { get; set; }
}
