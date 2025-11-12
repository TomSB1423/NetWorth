using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the CacheMetadata entity.
/// </summary>
public class CacheMetadataConfiguration : IEntityTypeConfiguration<Entities.CacheMetadata>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Entities.CacheMetadata> builder)
    {
        builder.ToTable("cache_metadata");

        builder.HasKey(c => c.CacheKey);

        builder.Property(c => c.CacheKey)
            .HasColumnName("cache_key")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastRefreshedAt)
            .HasColumnName("last_refreshed_at")
            .IsRequired();

        builder.Property(c => c.Count)
            .HasColumnName("count")
            .IsRequired();

        // Index for efficient queries
        builder.HasIndex(c => c.LastRefreshedAt)
            .HasDatabaseName("IX_CacheMetadata_LastRefreshedAt");
    }
}
