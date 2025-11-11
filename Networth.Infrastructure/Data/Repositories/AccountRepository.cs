using Networth.Domain.Entities;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;

namespace Networth.Infrastructure.Data.Repositories;

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
}
