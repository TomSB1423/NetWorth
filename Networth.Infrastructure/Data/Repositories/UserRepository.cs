using Microsoft.EntityFrameworkCore;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for User entities.
/// </summary>
public class UserRepository(NetworthDbContext dbContext) : IUserRepository
{
    /// <inheritdoc />
    public async Task<UserInfo?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return null;
        }

        return new UserInfo
        {
            Id = user.Id,
            Name = user.Name,
            HasCompletedOnboarding = user.HasCompletedOnboarding,
        };
    }

    /// <inheritdoc />
    public async Task<(UserInfo User, bool IsNew)> CreateOrGetUserAsync(string userId, string name, CancellationToken cancellationToken = default)
    {
        var existingUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (existingUser != null)
        {
            return (new UserInfo
            {
                Id = existingUser.Id,
                Name = existingUser.Name,
                HasCompletedOnboarding = existingUser.HasCompletedOnboarding,
            }, false);
        }

        var newUser = new User
        {
            Id = userId,
            Name = name,
            HasCompletedOnboarding = false,
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (new UserInfo
        {
            Id = newUser.Id,
            Name = newUser.Name,
            HasCompletedOnboarding = newUser.HasCompletedOnboarding,
        }, true);
    }

    /// <inheritdoc />
    public async Task<UserInfo?> UpdateUserAsync(
        string userId,
        string? name,
        bool? hasCompletedOnboarding,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return null;
        }

        if (name != null)
        {
            user.Name = name;
        }

        if (hasCompletedOnboarding.HasValue)
        {
            user.HasCompletedOnboarding = hasCompletedOnboarding.Value;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserInfo
        {
            Id = user.Id,
            Name = user.Name,
            HasCompletedOnboarding = user.HasCompletedOnboarding,
        };
    }
}
