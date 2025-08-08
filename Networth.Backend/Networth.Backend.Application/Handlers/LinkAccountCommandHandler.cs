using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Application.Handlers;

/// <summary>
///     Handler for link account commands that creates both agreement and requisition.
/// </summary>
public class LinkAccountCommandHandler(IFinancialProvider financialProvider, ILogger<LinkAccountCommandHandler> logger)
{
    public async Task<LinkAccountCommandResult> HandleAsync(LinkAccountCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting account link process for institution {InstitutionId}.", command.InstitutionId);

        Institution institutions = await financialProvider.GetInstitutionAsync(command.InstitutionId, cancellationToken);

        var agreement = await financialProvider.CreateAgreementAsync(
            command.InstitutionId,
            institutions.TransactionTotalDays,
            institutions.MaxAccessValidForDays,
            cancellationToken);

        logger.LogInformation(
            "Successfully created agreement {AgreementId} for institution {InstitutionId}",
            agreement.Id,
            command.InstitutionId);

        var requisition = await financialProvider.CreateRequisitionAsync(
            command.InstitutionId,
            agreement.Id,
            "https://example.com/callback", // Replace with actual redirect URL
            cancellationToken);

        logger.LogInformation(
            "Successfully created requisition {RequisitionId} for institution {InstitutionId}",
            requisition.Id,
            command.InstitutionId);

        return new LinkAccountCommandResult
        {
            AuthorizationLink = requisition.AuthorizationLink, Status = requisition.Status,
        };
    }
}
