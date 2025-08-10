using Microsoft.EntityFrameworkCore;
using Networth.Backend.Domain.Entities;
using Networth.Backend.Domain.Repositories;
using Networth.Backend.Infrastructure.Data.Context;

namespace Networth.Backend.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for Account entities.
///     Simplified to focus on essential account operations.
/// </summary>
public class AccountRepository : BaseRepository<Account, string>, IAccountRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AccountRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AccountRepository(NetworthDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Account>> GetAccountsByInstitutionIdAsync(string institutionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.InstitutionId == institutionId)
            .ToListAsync(cancellationToken);
    }
}
