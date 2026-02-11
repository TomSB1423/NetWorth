using Microsoft.Extensions.Logging;
using Networth.Application.Commands;
using Networth.Application.Interfaces;
using Networth.Domain.Entities;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for <see cref="UpdateAccountCommand"/>.
///     Updates an account's user-defined fields.
/// </summary>
public class UpdateAccountCommandHandler(
    IAccountRepository accountRepository,
    ILogger<UpdateAccountCommandHandler> logger) : IRequestHandler<UpdateAccountCommand, UserAccount>
{
    /// <inheritdoc />
    public async Task<UserAccount> HandleAsync(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating account {AccountId} for user {UserId}",
            command.AccountId,
            command.UserId);

        var updatedAccount = await accountRepository.UpdateAccountAsync(
            command.AccountId,
            command.UserId,
            command.DisplayName,
            command.Category,
            cancellationToken);

        if (updatedAccount == null)
        {
            logger.LogWarning("Account {AccountId} not found for user {UserId}", command.AccountId, command.UserId);
            throw new InvalidOperationException($"Account {command.AccountId} not found");
        }

        logger.LogInformation("Updated account {AccountId}", command.AccountId);

        return updatedAccount;
    }
}
