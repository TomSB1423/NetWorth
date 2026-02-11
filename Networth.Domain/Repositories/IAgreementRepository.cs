using Networth.Domain.Entities;

namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for Agreement entities.
/// </summary>
public interface IAgreementRepository : IBaseRepository<Agreement, string>
{
    /// <summary>
    ///     Saves an agreement to the database.
    /// </summary>
    /// <param name="agreement">The agreement domain entity.</param>
    /// <param name="userId">The user ID who owns this agreement.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task SaveAgreementAsync(Agreement agreement, Guid userId, CancellationToken cancellationToken = default);
}
