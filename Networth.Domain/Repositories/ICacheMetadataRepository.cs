using Networth.Domain.Entities;

namespace Networth.Domain.Repositories;

/// <summary>
///     Repository interface for cache metadata operations.
/// </summary>
public interface ICacheMetadataRepository : IBaseRepository<CacheMetadata, string>
{
    /// <summary>
    ///     Gets the cache key for institutions.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <returns>The cache key.</returns>
    string GetInstitutionsCacheKey(string countryCode);

    /// <summary>
    ///     Gets cache metadata by cache key.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cache metadata if found, otherwise null.</returns>
    Task<CacheMetadata?> GetByCacheKeyAsync(string cacheKey, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Upserts (updates or inserts) cache metadata.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="count">The count of items.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upserted cache metadata.</returns>
    Task<CacheMetadata> UpsertAsync(string cacheKey, int count, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if the cache is fresh (less than specified days old).
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="maxAgeDays">Maximum age in days.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if cache is fresh, otherwise false.</returns>
    Task<bool> IsCacheFreshAsync(string cacheKey, int maxAgeDays, CancellationToken cancellationToken = default);
}
