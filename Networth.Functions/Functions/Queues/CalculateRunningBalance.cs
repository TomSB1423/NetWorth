using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.ServiceDefaults;

namespace Networth.Functions.Functions.Queues;

/// <summary>
///     Azure Function that processes calculate running balance messages from the queue.
/// </summary>
public class CalculateRunningBalance(
    ILogger<CalculateRunningBalance> logger,
    IMediator mediator)
{
    /// <summary>
    ///     Processes calculate running balance messages from the queue.
    /// </summary>
    /// <param name="message">The queue message containing the command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [Function("CalculateRunningBalance")]
    public async Task RunAsync(
        [QueueTrigger(ResourceNames.CalculateRunningBalanceQueue, Connection = ResourceNames.Queues)]
        string message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing calculate running balance message: {Message}", message);

        CalculateRunningBalanceCommand? command;
        try
        {
            command = JsonSerializer.Deserialize<CalculateRunningBalanceCommand>(message);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize message: {Message}", message);
            throw;
        }

        if (command == null)
        {
             logger.LogError("Deserialized command is null for message: {Message}", message);
             return;
        }

        await mediator.Send<CalculateRunningBalanceCommand, CalculateRunningBalanceCommandResult>(command, cancellationToken);

        logger.LogInformation("Finished calculating running balance for account {AccountId}", command.AccountId);
    }
}
