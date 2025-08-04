using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Application.Handlers;

/// <summary>
///     Handler for link account commands that creates both agreement and requisition.
/// </summary>
public class LinkAccountCommandHandler(IFinancialProvider financialProvider, ILogger<LinkAccountCommandHandler> logger)
    : ILinkAccountCommandHandler
{
    /// <inheritdoc />
    public async Task<LinkAccountCommandResult> HandleAsync(LinkAccountCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting account link process for institution {InstitutionId} with reference {Reference}",
            command.InstitutionId,
            command.Reference);

        // Step 1: Create the agreement
        logger.LogInformation("Creating agreement for institution {InstitutionId}", command.InstitutionId);
        Agreement agreement = await financialProvider.CreateAgreementAsync(
            command.InstitutionId,
            command.MaxHistoricalDays,
            command.AccessValidForDays,
            cancellationToken);

        logger.LogInformation(
            "Successfully created agreement {AgreementId} for institution {InstitutionId}",
            agreement.Id,
            command.InstitutionId);

        // Step 2: Create the requisition using the agreement
        logger.LogInformation(
            "Creating requisition for institution {InstitutionId} with agreement {AgreementId}",
            command.InstitutionId,
            agreement.Id);

        Requisition requisition = await financialProvider.CreateRequisitionAsync(
            command.RedirectUrl,
            command.InstitutionId,
            agreement.Id,
            command.Reference,
            command.UserLanguage,
            cancellationToken);

        logger.LogInformation(
            "Successfully created requisition {RequisitionId} for institution {InstitutionId}",
            requisition.Id,
            command.InstitutionId);

        logger.LogInformation(
            "Account link process completed successfully. Agreement: {AgreementId}, Requisition: {RequisitionId}",
            agreement.Id,
            requisition.Id);

        return new LinkAccountCommandResult
        {
            Agreement = agreement,
            Requisition = requisition,
        };
    }
}
