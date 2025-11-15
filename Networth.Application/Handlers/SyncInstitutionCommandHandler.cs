using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Enums;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for syncing all accounts of an institution.
/// </summary>
public class SyncInstitutionCommandHandler(
    IRequisitionRepository requisitionRepository,
    IAccountRepository accountRepository,
    IFinancialProvider financialProvider,
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

        // Get requisitions for this institution and user from database
        var requisitions = await requisitionRepository.GetRequisitionsByInstitutionAndUserAsync(
            request.InstitutionId,
            request.UserId,
            cancellationToken);

        var requisition = requisitions.FirstOrDefault();

        if (requisition == null)
        {
            logger.LogWarning(
                "No requisition found for institution {InstitutionId} and user {UserId}",
                request.InstitutionId,
                request.UserId);

            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = 0,
                AccountIds = [],
            };
        }

        // Fetch latest requisition status from GoCardless
        var latestRequisition = await financialProvider.GetRequisitionAsync(requisition.Id, cancellationToken);

        if (latestRequisition == null)
        {
            logger.LogWarning(
                "Requisition {RequisitionId} not found in GoCardless",
                requisition.Id);

            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = 0,
                AccountIds = [],
            };
        }

        // Check if requisition is still pending
        if (latestRequisition.Status == AccountLinkStatus.Pending)
        {
            logger.LogInformation(
                "Requisition {RequisitionId} is still pending with status {Status}",
                requisition.Id,
                latestRequisition.Status);

            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = 0,
                AccountIds = [],
            };
        }

        // Update database with latest requisition from GoCardless
        await requisitionRepository.UpdateRequisitionAsync(latestRequisition, cancellationToken);

        logger.LogInformation(
            "Updated requisition {RequisitionId} with status {Status} and {AccountCount} accounts",
            latestRequisition.Id,
            latestRequisition.Status,
            latestRequisition.Accounts.Length);

        // Process accounts if linked
        if (latestRequisition.Status == AccountLinkStatus.Linked && latestRequisition.Accounts.Length > 0)
        {
            var accountIds = new List<string>();

            foreach (var accountId in latestRequisition.Accounts)
            {
                // Fetch account details from GoCardless
                var accountDetails = await financialProvider.GetAccountDetailsAsync(accountId, cancellationToken);

                if (accountDetails != null)
                {
                    // Store/update account in database
                    await accountRepository.UpsertAccountAsync(
                        accountId,
                        request.UserId,
                        latestRequisition.Id,
                        request.InstitutionId,
                        accountDetails,
                        cancellationToken);

                    // Enqueue sync for transactions and balances
                    await queueService.EnqueueAccountSyncAsync(
                        accountId,
                        request.UserId,
                        request.DateFrom,
                        request.DateTo,
                        cancellationToken);

                    accountIds.Add(accountId);

                    logger.LogInformation(
                        "Enqueued account {AccountId} for sync",
                        accountId);
                }
                else
                {
                    logger.LogWarning(
                        "Could not fetch details for account {AccountId}",
                        accountId);
                }
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

        logger.LogInformation(
            "Requisition {RequisitionId} has status {Status} with no linked accounts",
            latestRequisition.Id,
            latestRequisition.Status);

        return new SyncInstitutionCommandResult
        {
            InstitutionId = request.InstitutionId,
            AccountsEnqueued = 0,
            AccountIds = [],
        };
    }
}
