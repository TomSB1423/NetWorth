using System.Text.Json;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using DomainAgreement = Networth.Domain.Entities.Agreement;
using InfrastructureAgreement = Networth.Infrastructure.Data.Entities.Agreement;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for Agreement entities.
/// </summary>
public class AgreementRepository : BaseRepository<DomainAgreement, string>, IAgreementRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AgreementRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AgreementRepository(NetworthDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task SaveAgreementAsync(DomainAgreement agreement, string userId, CancellationToken cancellationToken = default)
    {
        var entity = new InfrastructureAgreement
        {
            Id = agreement.Id,
            UserId = userId,
            InstitutionId = agreement.InstitutionId,
            MaxHistoricalDays = agreement.MaxHistoricalDays,
            AccessValidForDays = agreement.AccessValidForDays,
            AccessScope = JsonSerializer.Serialize(agreement.AccessScope),
            Reconfirmation = false,
            Created = agreement.Created,
            Accepted = agreement.Accepted,
        };

        await Context.Agreements.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
