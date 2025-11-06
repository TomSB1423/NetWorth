using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the Institution entity.
/// </summary>
public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.ToTable("Institutions");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.OwnerId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.GoCardlessId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.RequisitionId)
            .HasMaxLength(200)
            .IsRequired();

        // Relationships
        builder.HasOne(i => i.Owner)
            .WithMany(u => u.Institutions)
            .HasForeignKey(i => i.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Requisition)
            .WithMany()
            .HasForeignKey(i => i.RequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Accounts)
            .WithOne(a => a.Institution)
            .HasForeignKey(a => a.InstitutionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(i => i.OwnerId)
            .HasDatabaseName("IX_Institutions_OwnerId");

        builder.HasIndex(i => i.GoCardlessId)
            .HasDatabaseName("IX_Institutions_GoCardlessId");
    }
}

