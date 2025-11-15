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

    /// <summary>
    ///     Upserts an account to the database.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="userId">The user ID who owns the account.</param>
    /// <param name="requisitionId">The requisition ID that created this account.</param>
    /// <param name="institutionId">The institution ID.</param>
    /// <param name="accountDetails">The detailed account information from GoCardless.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task UpsertAccountAsync(
        string accountId,
        string userId,
        string requisitionId,
        string institutionId,
        AccountDetail accountDetails,
        CancellationToken cancellationToken = default);
}
