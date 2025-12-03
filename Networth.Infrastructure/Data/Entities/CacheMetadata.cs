namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Entity representing cache metadata for tracking data freshness.
/// </summary>
public class CacheMetadata
{
    /// <summary>
    ///     Gets or sets the cache key identifier (e.g., 'institutions', 'accounts', 'transactions').
    /// </summary>
    public required string CacheKey { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp when this cache was last refreshed.
    /// </summary>
    public required DateTime LastRefreshedAt { get; set; }

    /// <summary>
    ///     Gets or sets the count of items in the cache.
    /// </summary>
    public required int Count { get; set; }
}
