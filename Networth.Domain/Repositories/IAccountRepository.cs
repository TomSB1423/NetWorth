using Networth.Domain.Entities;

namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for Account entities.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    ///     Gets all accounts for a specific user with institution information.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of user accounts with institution information.</returns>
    Task<IEnumerable<UserAccount>> GetAccountsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
