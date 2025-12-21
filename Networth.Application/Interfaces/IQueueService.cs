namespace Networth.Application.Interfaces;

/// <summary>
///     Interface for queue service operations.
/// </summary>
public interface IQueueService
{
    /// <summary>
    ///     Enqueues a message to trigger account synchronization.
    /// </summary>
    /// <param name="accountId">The account ID to synchronize.</param>
    /// <param name="userId">The user ID who owns the account.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnqueueAccountSyncAsync(
        string accountId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Enqueues a message to trigger institution synchronization.
    /// </summary>
    /// <param name="institutionId">The institution ID to synchronize.</param>
    /// <param name="userId">The user ID who owns the institution accounts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnqueueInstitutionSyncAsync(
        string institutionId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Enqueues a message to trigger running balance calculation.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnqueueCalculateRunningBalanceAsync(
        string accountId,
        CancellationToken cancellationToken = default);
}
