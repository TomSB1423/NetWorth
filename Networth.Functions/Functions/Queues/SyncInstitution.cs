using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Functions.Models.Requests;
using Networth.ServiceDefaults;

namespace Networth.Functions.Functions.Queues;

/// <summary>
///     Azure Function that processes institution sync messages from the queue.
/// </summary>
public class SyncInstitution(
    IMediator mediator,
    ILogger<SyncInstitution> logger)
{
    /// <summary>
    ///     Processes institution sync messages from the queue.
    /// </summary>
    /// <param name="message">The queue message containing sync parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [Function("SyncInstitutionQueue")]
    public async Task RunAsync(
        [QueueTrigger(ResourceNames.InstitutionSyncQueue, Connection = ResourceNames.Queues)]
        string message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing institution sync message: {Message}", message);

        SyncInstitutionMessage syncMessage = JsonSerializer.Deserialize<SyncInstitutionMessage>(message)
                                             ?? throw new InvalidOperationException("Failed to deserialize sync message");

        var command = new SyncInstitutionCommand
        {
            InstitutionId = syncMessage.InstitutionId, UserId = syncMessage.UserId,
        };

        await mediator.Send<SyncInstitutionCommand, SyncInstitutionCommandResult>(command, cancellationToken);

        logger.LogInformation(
            "Successfully processed institution sync for institution {InstitutionId}",
            syncMessage.InstitutionId);
    }
}
