using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using DomainCacheMetadata = Networth.Domain.Entities.CacheMetadata;
using InfrastructureCacheMetadata = Networth.Infrastructure.Data.Entities.CacheMetadata;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for cache metadata operations.
/// </summary>
public class CacheMetadataRepository : BaseRepository<InfrastructureCacheMetadata, string>, ICacheMetadataRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CacheMetadataRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CacheMetadataRepository(NetworthDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<DomainCacheMetadata?> GetByCacheKeyAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FirstOrDefaultAsync(c => c.CacheKey == cacheKey, cancellationToken);
        return entity == null ? null : MapToDomain(entity);
    }

    /// <inheritdoc />
    public async Task<DomainCacheMetadata> UpsertAsync(string cacheKey, int count, CancellationToken cancellationToken = default)
    {
        var existing = await DbSet.FirstOrDefaultAsync(c => c.CacheKey == cacheKey, cancellationToken);

        if (existing != null)
        {
            existing.LastRefreshedAt = DateTime.UtcNow;
            existing.Count = count;
            DbSet.Update(existing);
        }
        else
        {
            existing = new InfrastructureCacheMetadata
            {
                CacheKey = cacheKey,
                LastRefreshedAt = DateTime.UtcNow,
                Count = count,
            };
            await DbSet.AddAsync(existing, cancellationToken);
        }

        await Context.SaveChangesAsync(cancellationToken);

        return MapToDomain(existing);
    }

    /// <inheritdoc />
    public async Task<bool> IsCacheFreshAsync(string cacheKey, int maxAgeDays, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FirstOrDefaultAsync(c => c.CacheKey == cacheKey, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        var ageInDays = (DateTime.UtcNow - entity.LastRefreshedAt).TotalDays;
        return ageInDays < maxAgeDays;
    }

    // Override base methods to return domain entities
    async Task<DomainCacheMetadata?> IBaseRepository<DomainCacheMetadata, string>.GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        return entity == null ? null : MapToDomain(entity);
    }

    async Task<IEnumerable<DomainCacheMetadata>> IBaseRepository<DomainCacheMetadata, string>.GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await GetAllAsync(cancellationToken);
        return entities.Select(MapToDomain);
    }

    async Task<IEnumerable<DomainCacheMetadata>> IBaseRepository<DomainCacheMetadata, string>.FindAsync(Expression<Func<DomainCacheMetadata, bool>> predicate, CancellationToken cancellationToken)
    {
        // This is a simplified implementation - a full implementation would need to translate the expression
        var entities = await GetAllAsync(cancellationToken);
        var domainEntities = entities.Select(MapToDomain);
        return domainEntities.Where(predicate.Compile());
    }

    async Task<DomainCacheMetadata?> IBaseRepository<DomainCacheMetadata, string>.FirstOrDefaultAsync(Expression<Func<DomainCacheMetadata, bool>> predicate, CancellationToken cancellationToken)
    {
        // This is a simplified implementation - a full implementation would need to translate the expression
        var entities = await GetAllAsync(cancellationToken);
        var domainEntities = entities.Select(MapToDomain);
        return domainEntities.FirstOrDefault(predicate.Compile());
    }

    async Task<DomainCacheMetadata> IBaseRepository<DomainCacheMetadata, string>.AddAsync(DomainCacheMetadata entity, CancellationToken cancellationToken)
    {
        var infrastructureEntity = MapToInfrastructure(entity);
        var result = await AddAsync(infrastructureEntity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return MapToDomain(result);
    }

    Task IBaseRepository<DomainCacheMetadata, string>.AddRangeAsync(IEnumerable<DomainCacheMetadata> entities, CancellationToken cancellationToken)
    {
        var infrastructureEntities = entities.Select(MapToInfrastructure);
        return AddRangeAsync(infrastructureEntities, cancellationToken);
    }

    Task IBaseRepository<DomainCacheMetadata, string>.UpdateAsync(DomainCacheMetadata entity, CancellationToken cancellationToken)
    {
        var infrastructureEntity = MapToInfrastructure(entity);
        return UpdateAsync(infrastructureEntity, cancellationToken);
    }

    Task IBaseRepository<DomainCacheMetadata, string>.RemoveAsync(DomainCacheMetadata entity, CancellationToken cancellationToken)
    {
        var infrastructureEntity = MapToInfrastructure(entity);
        return RemoveAsync(infrastructureEntity, cancellationToken);
    }

    async Task<bool> IBaseRepository<DomainCacheMetadata, string>.AnyAsync(Expression<Func<DomainCacheMetadata, bool>> predicate, CancellationToken cancellationToken)
    {
        // This is a simplified implementation
        var entities = await GetAllAsync(cancellationToken);
        var domainEntities = entities.Select(MapToDomain);
        return domainEntities.Any(predicate.Compile());
    }

    async Task<int> IBaseRepository<DomainCacheMetadata, string>.CountAsync(Expression<Func<DomainCacheMetadata, bool>> predicate, CancellationToken cancellationToken)
    {
        // This is a simplified implementation
        var entities = await GetAllAsync(cancellationToken);
        var domainEntities = entities.Select(MapToDomain);
        return domainEntities.Count(predicate.Compile());
    }

    private static DomainCacheMetadata MapToDomain(InfrastructureCacheMetadata entity) =>
        new()
        {
            CacheKey = entity.CacheKey,
            LastRefreshedAt = entity.LastRefreshedAt,
            Count = entity.Count,
        };

    private static InfrastructureCacheMetadata MapToInfrastructure(DomainCacheMetadata domain) =>
        new()
        {
            CacheKey = domain.CacheKey,
            LastRefreshedAt = domain.LastRefreshedAt,
            Count = domain.Count,
        };
}
