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
    ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand, UserInfo>
{
    /// <inheritdoc />
    public async Task<UserInfo> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var updatedUser = await userRepository.UpdateUserAsync(
            command.UserId,
            command.Name,
            command.HasCompletedOnboarding,
            cancellationToken);

        if (updatedUser == null)
        {
            logger.LogWarning("User with ID {UserId} not found for update", command.UserId);
            throw new InvalidOperationException($"User with ID {command.UserId} not found");
        }

        logger.LogInformation("Updated user {UserId}", updatedUser.Id);

        return updatedUser;
    }
}
