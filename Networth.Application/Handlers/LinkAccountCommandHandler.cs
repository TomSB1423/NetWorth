using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Entities;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for link account commands that creates both agreement and requisition.
/// </summary>
public class LinkAccountCommandHandler(IFinancialProvider financialProvider, ILogger<LinkAccountCommandHandler> logger)
    : IRequestHandler<LinkAccountCommand, LinkAccountCommandResult>
{
    public async Task<LinkAccountCommandResult> HandleAsync(LinkAccountCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting account link process for institution {InstitutionId}.", command.InstitutionId);

        InstitutionMetadata institutionMetadata = await financialProvider.GetInstitutionAsync(command.InstitutionId, cancellationToken);

        var agreement = await financialProvider.CreateAgreementAsync(
            command.InstitutionId,
            institutionMetadata.TransactionTotalDays,
            institutionMetadata.MaxAccessValidForDays,
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
            AuthorizationLink = requisition.AuthenticationLink, Status = requisition.Status,
        };
    }
}
