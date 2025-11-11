using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Infrastructure.Data.Context;

namespace Networth.Infrastructure.Services;

/// <summary>
///     Service for managing user accounts stored in the database.
/// </summary>
public class AccountService(NetworthDbContext dbContext, ILogger<AccountService> logger) : IAccountService
{
    /// <inheritdoc />
    public async Task<IEnumerable<AccountDto>> GetAccountsByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving accounts for user {UserId}", userId);

        var accounts = await dbContext.Accounts
            .Where(a => a.OwnerId == userId)
            .Include(a => a.Institution)
            .OrderBy(a => a.Name)
            .Select(a => new AccountDto
            {
                Id = a.Id,
                InstitutionId = a.InstitutionId,
                Name = a.Name,
                Institution = new InstitutionInfo
                {
                    Id = a.Institution.Id,
                    GoCardlessId = a.Institution.GoCardlessId
                }
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Successfully retrieved {Count} accounts for user {UserId}",
            accounts.Count,
            userId);

        return accounts;
    }
}
