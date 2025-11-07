using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the Transaction entity.
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.OwnerId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.AccountId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.Time)
            .IsRequired();

        // Relationships
        builder.HasOne(t => t.Owner)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.OwnerId)
            .HasDatabaseName("IX_Transactions_OwnerId");

        builder.HasIndex(t => t.AccountId)
            .HasDatabaseName("IX_Transactions_AccountId");

        builder.HasIndex(t => t.Time)
            .HasDatabaseName("IX_Transactions_Time");
    }
}

