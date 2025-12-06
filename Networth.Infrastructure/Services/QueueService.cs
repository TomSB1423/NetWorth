using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;

namespace Networth.Infrastructure.Services;

/// <summary>
///     Service for managing Azure Storage Queue operations.
/// </summary>
public class QueueService : IQueueService
{
    private const string AccountSyncQueueName = "account-sync";
    private const string CalculateRunningBalanceQueueName = "calculate-running-balance";
    private readonly QueueServiceClient _queueServiceClient;
    private readonly ILogger<QueueService> _logger;

    public QueueService(QueueServiceClient queueServiceClient, ILogger<QueueService> logger)
    {
        _logger = logger;
        _queueServiceClient = queueServiceClient;
    }

    /// <inheritdoc />
    public async Task EnqueueAccountSyncAsync(
        string accountId,
        string userId,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Enqueueing account sync for account {AccountId}, user {UserId}",
            accountId,
            userId);

        var queueClient = _queueServiceClient.GetQueueClient(AccountSyncQueueName);

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

        // Manually encode as Base64 to match Azure Functions default expectation
        var messageBytes = System.Text.Encoding.UTF8.GetBytes(messageJson);
        var base64Message = Convert.ToBase64String(messageBytes);

        await queueClient.SendMessageAsync(base64Message, cancellationToken);

        _logger.LogInformation(
            "Successfully enqueued account sync for account {AccountId}",
            accountId);
    }

    /// <inheritdoc />
    public async Task EnqueueCalculateRunningBalanceAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enqueueing calculate running balance for account {AccountId}", accountId);

        var queueClient = _queueServiceClient.GetQueueClient(CalculateRunningBalanceQueueName);
        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var message = new { AccountId = accountId };
        var messageJson = JsonSerializer.Serialize(message);

        var messageBytes = System.Text.Encoding.UTF8.GetBytes(messageJson);
        var base64Message = Convert.ToBase64String(messageBytes);

        await queueClient.SendMessageAsync(base64Message, cancellationToken);

        _logger.LogInformation("Successfully enqueued calculate running balance for account {AccountId}", accountId);
    }
}
