namespace Networth.Domain.Entities;

/// <summary>
///     Represents cache metadata for tracking data freshness.
/// </summary>
public record CacheMetadata
{
    /// <summary>
    ///     Gets the cache key identifier (e.g., 'institutions', 'accounts', 'transactions').
    /// </summary>
    public required string CacheKey { get; init; }

    /// <summary>
    ///     Gets the timestamp when this cache was last refreshed.
    /// </summary>
    public required DateTime LastRefreshedAt { get; init; }

    /// <summary>
    ///     Gets the count of items in the cache.
    /// </summary>
    public required int Count { get; init; }
}
