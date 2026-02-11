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
    public async Task<UserInfo?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user is null ? null : MapToUserInfo(user);
    }

    /// <inheritdoc />
    public async Task<UserInfo?> GetUserByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, cancellationToken);

        return user is null ? null : MapToUserInfo(user);
    }

    /// <inheritdoc />
    public async Task<(UserInfo User, bool IsNew)> CreateOrGetUserAsync(
        string firebaseUid,
        string name,
        string email,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, cancellationToken);

        if (existingUser != null)
        {
            return (MapToUserInfo(existingUser), false);
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirebaseUid = firebaseUid,
            Name = name,
            Email = email,
            HasCompletedOnboarding = false,
            CreatedAt = DateTime.UtcNow,
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (MapToUserInfo(newUser), true);
    }

    /// <inheritdoc />
    public async Task<UserInfo?> UpdateUserAsync(
        Guid userId,
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

        return MapToUserInfo(user);
    }

    private static UserInfo MapToUserInfo(User user)
    {
        return new UserInfo
        {
            Id = user.Id,
            FirebaseUid = user.FirebaseUid,
            Name = user.Name,
            Email = user.Email,
            HasCompletedOnboarding = user.HasCompletedOnboarding,
        };
    }
}
