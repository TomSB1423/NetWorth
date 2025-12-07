using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Networth.ServiceDefaults;

namespace Networth.Functions.Extensions;

public static class QueueExtensions
{
    public static async Task EnsureQueuesCreatedAsync(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<QueueServiceClient>>();
        var queueServiceClient = serviceProvider.GetRequiredService<QueueServiceClient>();

        var queues = new[]
        {
            ResourceNames.AccountSyncQueue,
            ResourceNames.CalculateRunningBalanceQueue
        };

        foreach (var queueName in queues)
        {
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            logger.LogInformation("Ensured queue '{QueueName}' exists", queueName);
        }
    }
}
