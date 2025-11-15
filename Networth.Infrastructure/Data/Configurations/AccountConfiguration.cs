using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Account = Networth.Infrastructure.Data.Entities.Account;

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
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.UserId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.RequisitionId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.InstitutionId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Iban)
            .HasMaxLength(34);

        builder.Property(a => a.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(a => a.Product)
            .HasMaxLength(200);

        builder.Property(a => a.CashAccountType)
            .HasMaxLength(50);

        builder.Property(a => a.AdditionalAccountData)
            .HasColumnType("jsonb");

        builder.Property(a => a.Created)
            .IsRequired();

        // Foreign Keys
        builder.HasOne<Entities.User>()
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Entities.Requisition>()
            .WithMany()
            .HasForeignKey(a => a.RequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Entities.InstitutionMetadata>()
            .WithMany()
            .HasForeignKey(a => a.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationships
        builder.HasMany(a => a.Transactions)
            .WithOne()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Balances)
            .WithOne()
            .HasForeignKey(b => b.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.RequisitionId);
        builder.HasIndex(a => a.InstitutionId);
        builder.HasIndex(a => a.Created);
    }
}
