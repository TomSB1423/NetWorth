using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Application.Options;
using Networth.Domain.Entities;
using Networth.Domain.Enums;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for link institution commands that creates both agreement and requisition.
/// </summary>
public class LinkInstitutionCommandHandler(
    IFinancialProvider financialProvider,
    IAgreementRepository agreementRepository,
    IRequisitionRepository requisitionRepository,
    IOptions<FrontendOptions> frontendOptions,
    ILogger<LinkInstitutionCommandHandler> logger)
    : IRequestHandler<LinkInstitutionCommand, LinkInstitutionCommandResult>
{
    public async Task<LinkInstitutionCommandResult> HandleAsync(LinkInstitutionCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting institution link process for user {UserId} and institution {InstitutionId}",
            command.UserId,
            command.InstitutionId);

        // Check for existing linked requisitions for this institution and user
        var existingLink = await CheckExistingLinkAsync(command.UserId, command.InstitutionId, cancellationToken);
        if (existingLink != null)
        {
            return existingLink;
        }

        var agreement = await CreateAndSaveAgreementAsync(command, cancellationToken);
        var requisition = await CreateAndSaveRequisitionAsync(command, agreement.Id, cancellationToken);

        return new LinkInstitutionCommandResult
        {
            AuthorizationLink = requisition.AuthenticationLink,
            Status = requisition.Status,
            IsAlreadyLinked = false,
        };
    }

    private async Task<LinkInstitutionCommandResult?> CheckExistingLinkAsync(
        Guid userId,
        string institutionId,
        CancellationToken cancellationToken)
    {
        var existingRequisitions = await requisitionRepository.GetRequisitionsByInstitutionAndUserAsync(
            institutionId,
            userId,
            cancellationToken);

        var existingList = existingRequisitions.ToList();

        // Find the most recent requisition that is linked (has accounts)
        var linkedRequisition = existingList
            .FirstOrDefault(r => r.Status == AccountLinkStatus.Linked && r.Accounts.Length > 0);

        if (linkedRequisition != null)
        {
            logger.LogInformation(
                "Institution {InstitutionId} is already linked for user {UserId} via requisition {RequisitionId}",
                institutionId,
                userId,
                linkedRequisition.Id);

            return new LinkInstitutionCommandResult
            {
                AuthorizationLink = null,
                Status = linkedRequisition.Status,
                IsAlreadyLinked = true,
                ExistingRequisitionId = linkedRequisition.Id,
            };
        }

        // Check for pending requisitions - user might have started but not completed the flow
        var pendingRequisition = existingList
            .FirstOrDefault(r => r.Status == AccountLinkStatus.Pending);

        if (pendingRequisition != null)
        {
            logger.LogInformation(
                "Found pending requisition {RequisitionId} for institution {InstitutionId} and user {UserId}. " +
                "Fetching latest status from provider.",
                pendingRequisition.Id,
                institutionId,
                userId);

            // Check if the pending requisition has been completed
            var latestStatus = await financialProvider.GetRequisitionAsync(pendingRequisition.Id, cancellationToken);

            if (latestStatus is { Status: AccountLinkStatus.Linked, Accounts.Length: > 0 })
            {
                // Update our local record
                await requisitionRepository.UpdateRequisitionAsync(latestStatus, cancellationToken);

                logger.LogInformation(
                    "Pending requisition {RequisitionId} has been linked with {AccountCount} accounts",
                    latestStatus.Id,
                    latestStatus.Accounts.Length);

                return new LinkInstitutionCommandResult
                {
                    AuthorizationLink = null,
                    Status = latestStatus.Status,
                    IsAlreadyLinked = true,
                    ExistingRequisitionId = latestStatus.Id,
                };
            }
        }

        return null;
    }

    private async Task<Domain.Entities.Agreement> CreateAndSaveAgreementAsync(LinkInstitutionCommand command, CancellationToken cancellationToken)
    {
        InstitutionMetadata institution = await financialProvider.GetInstitutionAsync(command.InstitutionId, cancellationToken);

        var agreement = await financialProvider.CreateAgreementAsync(
            command.InstitutionId,
            institution.TransactionTotalDays,
            institution.MaxAccessValidForDays,
            cancellationToken);

        logger.LogInformation(
            "Successfully created agreement {AgreementId} for institution {InstitutionId}",
            agreement.Id,
            command.InstitutionId);

        // Save agreement to database
        await agreementRepository.SaveAgreementAsync(agreement, command.UserId, cancellationToken);

        logger.LogInformation("Saved agreement {AgreementId} to database", agreement.Id);

        return agreement;
    }

    private async Task<Domain.Entities.Requisition> CreateAndSaveRequisitionAsync(LinkInstitutionCommand command, string agreementId, CancellationToken cancellationToken)
    {
        var frontendUrl = frontendOptions.Value.Url;
        var callbackUrl = $"{frontendUrl}?institutionId={command.InstitutionId}";

        var requisition = await financialProvider.CreateRequisitionAsync(
            command.InstitutionId,
            agreementId,
            callbackUrl, // Redirect to frontend callback page
            cancellationToken);

        logger.LogInformation(
            "Successfully created requisition {RequisitionId} for institution {InstitutionId}",
            requisition.Id,
            command.InstitutionId);

        // Save requisition to database
        await requisitionRepository.SaveRequisitionAsync(requisition, command.UserId, cancellationToken);

        logger.LogInformation("Saved requisition {RequisitionId} to database", requisition.Id);

        return requisition;
    }
}
