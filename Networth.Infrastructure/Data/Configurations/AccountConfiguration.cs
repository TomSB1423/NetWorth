using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Domain.Entities;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the Account entity.
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.OwnerId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.InstitutionId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        // Relationships
        builder.HasOne(a => a.Owner)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Institution)
            .WithMany(i => i.Accounts)
            .HasForeignKey(a => a.InstitutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.OwnerId)
            .HasDatabaseName("IX_Accounts_OwnerId");

        builder.HasIndex(a => a.InstitutionId)
            .HasDatabaseName("IX_Accounts_InstitutionId");
    }
}
