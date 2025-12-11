using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Enums;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for CalculateRunningBalanceCommand.
/// </summary>
public class CalculateRunningBalanceCommandHandler(
    ITransactionRepository transactionRepository,
    IAccountRepository accountRepository,
    ILogger<CalculateRunningBalanceCommandHandler> logger)
    : IRequestHandler<CalculateRunningBalanceCommand, CalculateRunningBalanceCommandResult>
{
    /// <inheritdoc />
    public async Task<CalculateRunningBalanceCommandResult> HandleAsync(
        CalculateRunningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        await accountRepository.UpdateAccountStatusAsync(request.AccountId, AccountLinkStatus.Calculating, cancellationToken);

        int count = await transactionRepository.CalculateRunningBalancesAsync(request.AccountId, cancellationToken);

        logger.LogInformation("Calculated running balance for {Count} transactions for account {AccountId}", count, request.AccountId);

        await accountRepository.UpdateAccountStatusAsync(request.AccountId, AccountLinkStatus.Linked, cancellationToken);

        return new CalculateRunningBalanceCommandResult
        {
            Success = true, ProcessedTransactions = count,
        };
    }
}
