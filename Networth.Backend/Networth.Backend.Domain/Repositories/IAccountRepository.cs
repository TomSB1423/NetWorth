using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Domain.Repositories;

/// <summary>
///     Repository interface for Account entities.
/// </summary>
public interface IAccountRepository : IBaseRepository<Account, string>
{
}
