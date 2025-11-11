using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for syncing all accounts of an institution.
/// </summary>
public class SyncInstitutionCommandHandler(
    IAccountRepository accountRepository,
    IQueueService queueService,
    ILogger<SyncInstitutionCommandHandler> logger)
    : IRequestHandler<SyncInstitutionCommand, SyncInstitutionCommandResult>
{
    /// <inheritdoc />
    public async Task<SyncInstitutionCommandResult> HandleAsync(
        SyncInstitutionCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling SyncInstitutionCommand for institution {InstitutionId}, user {UserId}",
            request.InstitutionId,
            request.UserId);

        // Get all user accounts
        var userAccounts = await accountRepository.GetAccountsByUserIdAsync(request.UserId, cancellationToken);

        // Filter accounts belonging to the specified institution
        var institutionAccounts = userAccounts
            .Where(a => a.InstitutionId == request.InstitutionId)
            .ToList();

        logger.LogInformation(
            "Found {Count} accounts for institution {InstitutionId}",
            institutionAccounts.Count,
            request.InstitutionId);

        // Enqueue sync for each account
        var accountIds = new List<string>();
        foreach (var account in institutionAccounts)
        {
            await queueService.EnqueueAccountSyncAsync(
                account.Id,
                request.UserId,
                request.DateFrom,
                request.DateTo,
                cancellationToken);

            accountIds.Add(account.Id);
        }

        logger.LogInformation(
            "Successfully enqueued {Count} accounts for sync from institution {InstitutionId}",
            accountIds.Count,
            request.InstitutionId);

        return new SyncInstitutionCommandResult
        {
            InstitutionId = request.InstitutionId,
            AccountsEnqueued = accountIds.Count,
            AccountIds = accountIds,
        };
    }
}
