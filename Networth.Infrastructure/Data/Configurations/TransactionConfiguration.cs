using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transaction = Networth.Infrastructure.Data.Entities.Transaction;

namespace Networth.Infrastructure.Data.Configurations;

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
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.AccountId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.TransactionId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.DebtorName)
            .HasMaxLength(500);

        builder.Property(t => t.DebtorAccountIban)
            .HasMaxLength(34);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.BankTransactionCode)
            .HasMaxLength(100);

        builder.Property(t => t.RemittanceInformationUnstructured)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasMaxLength(50);

        builder.Property(t => t.ImportedAt)
            .IsRequired();

        builder.Property(t => t.RunningBalance)
            .HasPrecision(18, 2);

        // Foreign Keys
        builder.HasOne<Entities.User>()
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Entities.Account>()
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.TransactionId);
        builder.HasIndex(t => t.BookingDate);
        builder.HasIndex(t => t.ValueDate);
        builder.HasIndex(t => t.ImportedAt);
    }
}
