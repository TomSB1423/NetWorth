using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the InstitutionMetadata entity.
/// </summary>
public class InstitutionMetadataConfiguration : IEntityTypeConfiguration<Entities.InstitutionMetadata>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Entities.InstitutionMetadata> builder)
    {
        builder.ToTable("Institutions");

        builder.HasKey(i => new { i.Id, i.CountryCode });

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        builder.Property(i => i.Bic)
            .HasColumnName("bic")
            .HasMaxLength(50);

        builder.Property(i => i.CountryCode)
            .HasColumnName("country_code")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(i => i.Countries)
            .HasColumnName("countries")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(i => i.SupportedFeatures)
            .HasColumnName("supported_features")
            .HasColumnType("jsonb");

        builder.Property(i => i.LastUpdated)
            .HasColumnName("last_updated")
            .IsRequired();

        // Indexes for efficient queries
        builder.HasIndex(i => i.CountryCode)
            .HasDatabaseName("IX_InstitutionMetadata_CountryCode");

        builder.HasIndex(i => i.LastUpdated)
            .HasDatabaseName("IX_InstitutionMetadata_LastUpdated");
    }
}
