using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using UserAccount = Networth.Domain.Entities.UserAccount;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for Account entities.
/// </summary>
public class AccountRepository(NetworthDbContext context, ILogger<AccountRepository> logger) : IAccountRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<UserAccount>> GetAccountsByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving accounts for user {UserId}", userId);

        var accounts = await context.Accounts
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Name)
            .Select(a => new UserAccount
            {
                Id = a.Id,
                UserId = a.UserId,
                RequisitionId = a.RequisitionId,
                InstitutionId = a.InstitutionId,
                Name = a.Name,
                Iban = a.Iban,
                Currency = a.Currency,
                Product = a.Product,
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Successfully retrieved {Count} accounts for user {UserId}",
            accounts.Count,
            userId);

        return accounts;
    }
}
