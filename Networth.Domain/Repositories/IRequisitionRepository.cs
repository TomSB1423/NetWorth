using Networth.Domain.Entities;

namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for Requisition entities.
/// </summary>
public interface IRequisitionRepository : IBaseRepository<Requisition, string>
{
    /// <summary>
    ///     Saves a requisition to the database.
    /// </summary>
    /// <param name="requisition">The requisition domain entity.</param>
    /// <param name="userId">The user ID who owns this requisition.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task SaveRequisitionAsync(Requisition requisition, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing requisition in the database.
    /// </summary>
    /// <param name="requisition">The updated requisition domain entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task UpdateRequisitionAsync(Requisition requisition, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a requisition by ID as a domain entity.
    /// </summary>
    /// <param name="requisitionId">The requisition ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requisition domain entity, or null if not found.</returns>
    Task<Requisition?> GetRequisitionByIdAsync(string requisitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets requisitions by institution ID and user ID.
    /// </summary>
    /// <param name="institutionId">The institution ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of requisitions for the institution and user.</returns>
    Task<IEnumerable<Requisition>> GetRequisitionsByInstitutionAndUserAsync(
        string institutionId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the institution IDs that a user has linked (has requisitions with Linked status and accounts).
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of institution IDs that are linked for the user.</returns>
    Task<IEnumerable<string>> GetLinkedInstitutionIdsForUserAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
