using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for <see cref="UpdateUserCommand"/>.
///     Updates user fields.
/// </summary>
public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand, UpdateUserCommandResult>
{
    /// <inheritdoc />
    public async Task<UpdateUserCommandResult> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var updatedUser = await userRepository.UpdateUserAsync(
            command.UserId,
            command.Name,
            command.HasCompletedOnboarding,
            cancellationToken);

        if (updatedUser == null)
        {
            logger.LogWarning("User {UserId} not found for update", command.UserId);
            throw new InvalidOperationException($"User {command.UserId} not found");
        }

        logger.LogInformation("Updated user {UserId}", command.UserId);

        return new UpdateUserCommandResult
        {
            UserId = updatedUser.Id,
            Name = updatedUser.Name,
            HasCompletedOnboarding = updatedUser.HasCompletedOnboarding,
        };
    }
}
