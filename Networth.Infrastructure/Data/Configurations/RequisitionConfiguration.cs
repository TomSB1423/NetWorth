using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Infrastructure.Data.Entities;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the Requisition entity.
/// </summary>
public class RequisitionConfiguration : IEntityTypeConfiguration<Requisition>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Requisition> builder)
    {
        builder.ToTable("Requisitions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.UserId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.Created)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.InstitutionId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.AgreementId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.Reference)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Accounts)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(r => r.UserLanguage)
            .HasMaxLength(10);

        builder.Property(r => r.Link)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.Ssn)
            .HasMaxLength(50);

        builder.Property(r => r.Redirect)
            .HasMaxLength(500);

        // Foreign Keys
        builder.HasOne<Entities.User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Note: InstitutionId is stored for reference but has no FK constraint
        // because InstitutionMetadata has a composite key (Id + CountryCode)
        // and we only store the Id portion

        builder.HasOne<Entities.Agreement>()
            .WithMany(a => a.Requisitions)
            .HasForeignKey(r => r.AgreementId)
            .OnDelete(DeleteBehavior.Restrict);

        // Create indexes for common queries
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.InstitutionId);
        builder.HasIndex(r => r.AgreementId);
        builder.HasIndex(r => r.Created);
    }
}
