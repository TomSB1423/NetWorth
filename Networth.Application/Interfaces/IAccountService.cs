namespace Networth.Application.Interfaces;

/// <summary>
///     Service for managing user accounts stored in the database.
/// </summary>
public interface IAccountService
{
    /// <summary>
    ///     Gets all accounts for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of accounts owned by the user.</returns>
    Task<IEnumerable<AccountDto>> GetAccountsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
