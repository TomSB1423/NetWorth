using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Entities;
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

        Requisition? latestRequisition = await GetRequisitionAsync(request.InstitutionId, request.UserId, cancellationToken);

        if (latestRequisition == null)
        {
            return new SyncInstitutionCommandResult
            {
                InstitutionId = request.InstitutionId,
                AccountsEnqueued = 0,
                AccountIds = [],
            };
        }

        if (latestRequisition is { Status: AccountLinkStatus.Linked, Accounts.Length: > 0 })
        {
            return await ProcessLinkedAccountsAsync(request, latestRequisition, cancellationToken);
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

    private async Task<Requisition?> GetRequisitionAsync(string institutionId, Guid userId, CancellationToken cancellationToken)
    {
        var requisitions = await requisitionRepository.GetRequisitionsByInstitutionAndUserAsync(
            institutionId,
            userId,
            cancellationToken);

        var requisitionsList = requisitions.ToList();
        logger.LogDebug(
            "Found {Count} requisition(s) for institution {InstitutionId}",
            requisitionsList.Count,
            institutionId);

        var requisition = requisitionsList.FirstOrDefault();

        if (requisition == null)
        {
            logger.LogWarning(
                "No requisition found for institution {InstitutionId} and user {UserId}. User needs to link their account first.",
                institutionId,
                userId);
            return null;
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
            return null;
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
            return null;
        }

        await requisitionRepository.UpdateRequisitionAsync(latestRequisition, cancellationToken);

        return latestRequisition;
    }

    private async Task<SyncInstitutionCommandResult> ProcessLinkedAccountsAsync(
        SyncInstitutionCommand request,
        Requisition latestRequisition,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Processing {Count} linked account(s)",
            latestRequisition.Accounts.Length);

        List<string> accountIds = [];
        int successCount = 0;
        int failureCount = 0;

        foreach (string accountId in latestRequisition.Accounts)
        {
            try
            {
                AccountDetail? accountDetails = await financialProvider.GetAccountDetailsAsync(accountId, cancellationToken);

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

        string failureInfo = failureCount > 0
            ? $", {failureCount} failed"
            : string.Empty;
        logger.LogInformation(
            "Enqueued {SuccessCount} account(s) for sync from institution {InstitutionId}{FailureInfo}",
            successCount,
            request.InstitutionId,
            failureInfo);

        return new SyncInstitutionCommandResult
        {
            InstitutionId = request.InstitutionId,
            AccountsEnqueued = accountIds.Count,
            AccountIds = accountIds,
        };
    }
}
