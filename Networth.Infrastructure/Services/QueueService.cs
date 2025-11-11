using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;

namespace Networth.Infrastructure.Services;

/// <summary>
///     Service for managing Azure Storage Queue operations.
/// </summary>
public class QueueService(QueueServiceClient queueServiceClient, ILogger<QueueService> logger) : IQueueService
{
    private const string AccountSyncQueueName = "account-sync";

    /// <inheritdoc />
    public async Task EnqueueAccountSyncAsync(
        string accountId,
        string userId,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Enqueueing account sync for account {AccountId}, user {UserId}",
            accountId,
            userId);

        var queueClient = queueServiceClient.GetQueueClient(AccountSyncQueueName);

        // Ensure queue exists
        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var message = new
        {
            AccountId = accountId,
            UserId = userId,
            DateFrom = dateFrom,
            DateTo = dateTo
        };

        var messageJson = JsonSerializer.Serialize(message);

        await queueClient.SendMessageAsync(messageJson, cancellationToken);

        logger.LogInformation(
            "Successfully enqueued account sync for account {AccountId}",
            accountId);
    }
}
