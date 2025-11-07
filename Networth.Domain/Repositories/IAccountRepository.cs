using Networth.Domain.Entities;

namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for Account entities.
/// </summary>
public interface IAccountRepository : IBaseRepository<Account, string>
{
}
