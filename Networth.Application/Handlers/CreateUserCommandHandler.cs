using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for <see cref="CreateUserCommand"/>.
///     Creates a new user or returns the existing user if already present.
/// </summary>
public class CreateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand, CreateUserCommandResult>
{
    /// <inheritdoc />
    public async Task<CreateUserCommandResult> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var (user, isNew) = await userRepository.CreateOrGetUserAsync(
            command.UserId,
            command.Name ?? "Unknown User",
            cancellationToken);

        if (isNew)
        {
            logger.LogInformation("Created new user {UserId} with name {Name}", user.Id, user.Name);
        }
        else
        {
            logger.LogDebug("User {UserId} already exists", user.Id);
        }

        return new CreateUserCommandResult
        {
            UserId = user.Id,
            Name = user.Name,
            IsNewUser = isNew,
        };
    }
}
