using Networth.Backend.Domain.Entities;
using Networth.Backend.Domain.Enums;

namespace Networth.Backend.Domain.Repositories;

/// <summary>
///     Repository interface for Requisition entities.
/// </summary>
public interface IRequisitionRepository : IBaseRepository<Requisition, string>
{
    /// <summary>
    ///     Gets requisitions by status.
    /// </summary>
    /// <param name="status">The requisition status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of requisitions with the specified status.</returns>
    Task<IEnumerable<Requisition>> GetRequisitionsByStatusAsync(AccountLinkStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets requisitions by user reference.
    /// </summary>
    /// <param name="userReference">The user reference identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of requisitions for the specified user.</returns>
    Task<IEnumerable<Requisition>> GetRequisitionsByUserReferenceAsync(string userReference, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets expired requisitions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of expired requisitions.</returns>
    Task<IEnumerable<Requisition>> GetExpiredRequisitionsAsync(CancellationToken cancellationToken = default);
}
