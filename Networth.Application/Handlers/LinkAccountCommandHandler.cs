using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Entities;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for link account commands that creates both agreement and requisition.
/// </summary>
public class LinkAccountCommandHandler(
    IFinancialProvider financialProvider,
    IAgreementRepository agreementRepository,
    IRequisitionRepository requisitionRepository,
    ILogger<LinkAccountCommandHandler> logger)
    : IRequestHandler<LinkAccountCommand, LinkAccountCommandResult>
{
    public async Task<LinkAccountCommandResult> HandleAsync(LinkAccountCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting account link process for user {UserId} and institution {InstitutionId}",
            command.UserId,
            command.InstitutionId);

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

        var requisition = await financialProvider.CreateRequisitionAsync(
            command.InstitutionId,
            agreement.Id,
            "https://example.com/callback", // Replace with actual redirect URL
            cancellationToken);

        logger.LogInformation(
            "Successfully created requisition {RequisitionId} for institution {InstitutionId}",
            requisition.Id,
            command.InstitutionId);

        // Save requisition to database
        await requisitionRepository.SaveRequisitionAsync(requisition, command.UserId, cancellationToken);

        logger.LogInformation("Saved requisition {RequisitionId} to database", requisition.Id);

        return new LinkAccountCommandResult
        {
            AuthorizationLink = requisition.AuthenticationLink, Status = requisition.Status,
        };
    }
}
