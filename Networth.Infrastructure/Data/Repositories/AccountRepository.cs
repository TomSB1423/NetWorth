using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Networth.Domain.Entities;
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

    /// <inheritdoc />
    public async Task UpsertAccountAsync(
        string accountId,
        string userId,
        string requisitionId,
        string institutionId,
        AccountDetail accountDetails,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Upserting account {AccountId} for user {UserId}",
            accountId,
            userId);

        var existingAccount = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (existingAccount != null)
        {
            // Update existing account
            existingAccount.Name = accountDetails.Name ?? existingAccount.Name;
            existingAccount.Currency = accountDetails.Currency ?? existingAccount.Currency;
            existingAccount.Product = accountDetails.Product;
            existingAccount.CashAccountType = accountDetails.CashAccountType;
            existingAccount.LastSynced = DateTime.UtcNow;

            logger.LogInformation("Updated existing account {AccountId}", accountId);
        }
        else
        {
            // Create new account
            var newAccount = new Entities.Account
            {
                Id = accountId,
                UserId = userId,
                RequisitionId = requisitionId,
                InstitutionId = institutionId,
                Name = accountDetails.Name ?? "Unknown Account",
                Currency = accountDetails.Currency ?? "GBP",
                Product = accountDetails.Product,
                CashAccountType = accountDetails.CashAccountType,
                Created = DateTime.UtcNow,
                LastSynced = DateTime.UtcNow,
            };

            await context.Accounts.AddAsync(newAccount, cancellationToken);

            logger.LogInformation("Created new account {AccountId}", accountId);
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully upserted account {AccountId}", accountId);
    }
}
