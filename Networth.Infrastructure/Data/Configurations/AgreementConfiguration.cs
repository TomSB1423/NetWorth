using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Configures the <see cref="Entities.Agreement" /> entity.
/// </summary>
public class AgreementConfiguration : IEntityTypeConfiguration<Entities.Agreement>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Entities.Agreement> builder)
    {
        builder.ToTable("Agreements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.InstitutionId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.AccessScope)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(a => a.Created)
            .IsRequired();

        // Foreign Keys
        builder.HasOne<Entities.User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Note: InstitutionId is stored for reference but has no FK constraint
        // because InstitutionMetadata has a composite key (Id + CountryCode)
        // and we only store the Id portion

        // Relationships
        builder.HasMany(a => a.Requisitions)
            .WithOne()
            .HasForeignKey("AgreementId")
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.InstitutionId);
        builder.HasIndex(a => a.Created);
    }
}
