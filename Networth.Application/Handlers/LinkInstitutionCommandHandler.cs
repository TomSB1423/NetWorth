using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Application.Options;
using Networth.Domain.Entities;
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

        var agreement = await CreateAndSaveAgreementAsync(command, cancellationToken);
        var requisition = await CreateAndSaveRequisitionAsync(command, agreement.Id, cancellationToken);

        return new LinkInstitutionCommandResult
        {
            AuthorizationLink = requisition.AuthenticationLink,
            Status = requisition.Status,
        };
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
