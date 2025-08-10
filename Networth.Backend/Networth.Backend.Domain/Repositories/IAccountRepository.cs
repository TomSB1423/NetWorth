using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Domain.Repositories;

/// <summary>
///     Repository interface for Account entities.
/// </summary>
public interface IAccountRepository : IBaseRepository<Account, string>
{
    /// <summary>
    ///     Gets accounts by institution ID.
    /// </summary>
    /// <param name="institutionId">The institution ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of accounts for the institution.</returns>
    Task<IEnumerable<Account>> GetAccountsByInstitutionIdAsync(string institutionId, CancellationToken cancellationToken = default);
}
