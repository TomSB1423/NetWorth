using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Domain.Entities;

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
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Created)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.InstitutionId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.AgreementId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Reference)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.AuthenticationLink)
            .HasMaxLength(500);

        builder.Property(r => r.AccountSelection)
            .HasMaxLength(100);

        // Create indexes for common queries
        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Requisitions_Status");
    }
}
