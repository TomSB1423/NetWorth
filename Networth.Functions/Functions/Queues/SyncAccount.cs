using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Functions.Models.Requests;
using Networth.ServiceDefaults;

namespace Networth.Functions.Functions.Queues;

/// <summary>
///     Azure Function that processes account sync messages from the queue.
/// </summary>
public class SyncAccount(
    ILogger<SyncAccount> logger,
    IMediator mediator,
    IQueueService queueService)
{
    /// <summary>
    ///     Processes account sync messages from the queue.
    /// </summary>
    /// <param name="message">The queue message containing sync parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [Function("SyncAccount")]
    public async Task RunAsync(
        [QueueTrigger(ResourceNames.AccountSyncQueue, Connection = ResourceNames.Queues)]
        string message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing account sync message: {Message}", message);

        SyncAccountMessage syncMessage = JsonSerializer.Deserialize<SyncAccountMessage>(message)
                                        ?? throw new SerializationException("Failed to deserialize sync account message.");

        SyncAccountCommand command = new()
        {
            AccountId = syncMessage.AccountId,
            UserId = syncMessage.UserId,
        };

        SyncAccountCommandResult result = await mediator.Send<SyncAccountCommand, SyncAccountCommandResult>(command, cancellationToken);

        logger.LogInformation(
            "Successfully synced {Count} transactions for account {AccountId} from {DateFrom} to {DateTo}",
            result.TransactionCount,
            result.AccountId,
            result.DateFrom,
            result.DateTo);

        await queueService.EnqueueCalculateRunningBalanceAsync(result.AccountId, cancellationToken);
    }
}
