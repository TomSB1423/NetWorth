using Networth.Application.Interfaces;

namespace Networth.Application.Commands;

/// <summary>
///     Command to calculate running balances for an account.
/// </summary>
public class CalculateRunningBalanceCommand : IRequest<CalculateRunningBalanceCommandResult>
{
    /// <summary>
    ///     Gets or sets the account ID.
    /// </summary>
    public required string AccountId { get; set; }
}
