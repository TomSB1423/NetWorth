using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using DomainRequisition = Networth.Domain.Entities.Requisition;
using InfrastructureRequisition = Networth.Infrastructure.Data.Entities.Requisition;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for Requisition entities.
/// </summary>
public class RequisitionRepository : BaseRepository<DomainRequisition, string>, IRequisitionRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequisitionRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RequisitionRepository(NetworthDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task SaveRequisitionAsync(DomainRequisition requisition, string userId, CancellationToken cancellationToken = default)
    {
        var entity = new InfrastructureRequisition
        {
            Id = requisition.Id,
            UserId = userId,
            InstitutionId = requisition.InstitutionId,
            AgreementId = requisition.AgreementId,
            Status = requisition.Status,
            Reference = requisition.Reference,
            Redirect = requisition.Redirect,
            Accounts = JsonSerializer.Serialize(requisition.Accounts),
            Link = requisition.AuthenticationLink,
            AccountSelection = !string.IsNullOrEmpty(requisition.AccountSelection),
            Created = requisition.Created,
        };

        await Context.Requisitions.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DomainRequisition?> GetRequisitionByIdAsync(string requisitionId, CancellationToken cancellationToken = default)
    {
        var entity = await Context.Requisitions
            .FirstOrDefaultAsync(r => r.Id == requisitionId, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return new DomainRequisition
        {
            Id = entity.Id,
            Created = entity.Created,
            Redirect = entity.Redirect,
            Status = entity.Status,
            InstitutionId = entity.InstitutionId,
            AgreementId = entity.AgreementId,
            Reference = entity.Reference,
            Accounts = JsonSerializer.Deserialize<string[]>(entity.Accounts) ?? [],
            AuthenticationLink = entity.Link,
            AccountSelection = entity.AccountSelection ? "true" : null,
        };
    }
}
