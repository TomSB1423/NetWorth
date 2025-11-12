using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Infrastructure.Data.Entities;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the Institution entity.
/// </summary>
public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.ToTable("UserInstitutions");

        builder.HasKey(ui => ui.Id);

        builder.Property(ui => ui.Id)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ui => ui.OwnerId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ui => ui.GoCardlessInstitutionId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ui => ui.RequisitionId)
            .HasMaxLength(200)
            .IsRequired();

        // Relationships
        builder.HasOne(ui => ui.Owner)
            .WithMany(u => u.UserInstitutions)
            .HasForeignKey(ui => ui.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ui => ui.Requisition)
            .WithMany()
            .HasForeignKey(ui => ui.RequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ui => ui.Accounts)
            .WithOne(a => a.Institution)
            .HasForeignKey(a => a.UserInstitutionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ui => ui.OwnerId)
            .HasDatabaseName("IX_UserInstitutions_OwnerId");

        builder.HasIndex(ui => ui.GoCardlessInstitutionId)
            .HasDatabaseName("IX_UserInstitutions_GoCardlessInstitutionId");
    }
}
