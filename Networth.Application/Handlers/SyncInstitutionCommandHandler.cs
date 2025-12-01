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
            "Syncing institution {InstitutionId} for user {UserId}",
            request.InstitutionId,
            request.UserId);

        var requisitions = await requisitionRepository.GetRequisitionsByInstitutionAndUserAsync(
            request.InstitutionId,
            request.UserId,
            cancellationToken);

        var requisitionsList = requisitions.ToList();
        logger.LogDebug(
            "Found {Count} requisition(s) for institution {InstitutionId}",
            requisitionsList.Count,
            request.InstitutionId);

        var requisition = requisitionsList.FirstOrDefault();

        if (requisition == null)
        {
            logger.LogWarning(
                "No requisition found for institution {InstitutionId} and user {UserId}. User needs to link their account first.",
                request.InstitutionId,
                request.UserId);

            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = 0,
                AccountIds = [],
            };
        }

        logger.LogDebug(
            "Using requisition {RequisitionId} with status {Status}",
            requisition.Id,
            requisition.Status);
        var latestRequisition = await financialProvider.GetRequisitionAsync(requisition.Id, cancellationToken);

        if (latestRequisition == null)
        {
            logger.LogError(
                "Requisition {RequisitionId} not found in GoCardless API. It may have been deleted.",
                requisition.Id);

            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = 0,
                AccountIds = [],
            };
        }

        logger.LogDebug(
            "Requisition {RequisitionId} status: {Status}, accounts: {AccountCount}",
            latestRequisition.Id,
            latestRequisition.Status,
            latestRequisition.Accounts.Length);
        if (latestRequisition.Status == AccountLinkStatus.Pending)
        {
            logger.LogWarning(
                "Requisition {RequisitionId} is still pending with status {Status}. User needs to complete authorization.",
                requisition.Id,
                latestRequisition.Status);

            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = 0,
                AccountIds = [],
            };
        }

        await requisitionRepository.UpdateRequisitionAsync(latestRequisition, cancellationToken);

        if (latestRequisition.Status == AccountLinkStatus.Linked && latestRequisition.Accounts.Length > 0)
        {
            logger.LogInformation(
                "Processing {Count} linked account(s)",
                latestRequisition.Accounts.Length);

            var accountIds = new List<string>();
            int successCount = 0;
            int failureCount = 0;

            foreach (var accountId in latestRequisition.Accounts)
            {
                try
                {
                    var accountDetails = await financialProvider.GetAccountDetailsAsync(accountId, cancellationToken);

                    if (accountDetails != null)
                    {
                        logger.LogDebug(
                            "Account details for {AccountId}: {Name}",
                            accountId,
                            accountDetails.DisplayName ?? accountDetails.Name);
                        await accountRepository.UpsertAccountAsync(
                            accountId,
                            request.UserId,
                            latestRequisition.Id,
                            request.InstitutionId,
                            accountDetails,
                            cancellationToken);

                        await queueService.EnqueueAccountSyncAsync(
                            accountId,
                            request.UserId,
                            request.DateFrom,
                            request.DateTo,
                            cancellationToken);

                        accountIds.Add(accountId);
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                        logger.LogError(
                            "Could not fetch details for account {AccountId} - received null response from GoCardless",
                            accountId);
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    logger.LogError(
                        ex,
                        "Error processing account {AccountId}: {ErrorMessage}",
                        accountId,
                        ex.Message);
                }
            }

            logger.LogInformation(
                "Enqueued {SuccessCount} account(s) for sync from institution {InstitutionId}{FailureInfo}",
                successCount,
                request.InstitutionId,
                failureCount > 0 ? $", {failureCount} failed" : "");

            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = accountIds.Count,
                AccountIds = accountIds,
            };
        }

        logger.LogWarning(
            "Requisition {RequisitionId} has status {Status} with no linked accounts to process",
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

