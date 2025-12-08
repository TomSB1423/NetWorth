using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Networth.Domain.Entities;
using Networth.Domain.Enums;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using Npgsql;
using Account = Networth.Infrastructure.Data.Entities.Account;
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
                LastSynced = a.LastSynced,
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

        var retries = 3;
        while (retries > 0)
        {
            try
            {
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
                    var newAccount = new Account
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
                return;
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyException(ex) && retries > 1)
            {
                logger.LogWarning(ex, "Concurrency conflict upserting account {AccountId}. Retrying...", accountId);
                context.ChangeTracker.Clear();
                retries--;
            }
        }
    }



    /// <inheritdoc />
    public async Task UpsertAccountBalancesAsync(
        string accountId,
        IEnumerable<AccountBalance> balances,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Upserting balances for account {AccountId}", accountId);

        foreach (var balance in balances)
        {
            var existingBalance = await context.AccountBalances
                .FirstOrDefaultAsync(
                    b => b.AccountId == accountId &&
                         b.BalanceType == balance.BalanceType &&
                         b.ReferenceDate == balance.ReferenceDate,
                    cancellationToken);

            if (existingBalance != null)
            {
                existingBalance.Amount = balance.Amount;
                existingBalance.Currency = balance.Currency;
                existingBalance.RetrievedAt = DateTime.UtcNow;
            }
            else
            {
                var newBalance = new Entities.AccountBalance
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = accountId,
                    BalanceType = balance.BalanceType,
                    Amount = balance.Amount,
                    Currency = balance.Currency,
                    ReferenceDate = balance.ReferenceDate,
                    RetrievedAt = DateTime.UtcNow,
                };
                await context.AccountBalances.AddAsync(newBalance, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully upserted balances for account {AccountId}", accountId);
    }

    /// <inheritdoc />
    public async Task UpdateAccountStatusAsync(string accountId, AccountLinkStatus status, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating status for account {AccountId} to {Status}", accountId, status);

        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
        if (account != null)
        {
            account.Status = status;
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Updated status for account {AccountId} to {Status}", accountId, status);
        }
        else
        {
            logger.LogWarning("Account {AccountId} not found, cannot update status", accountId);
        }
    }

    private static bool IsDuplicateKeyException(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505";
    }
}
