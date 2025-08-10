using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Infrastructure.Data.Configurations;

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

        builder.Property(a => a.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.InstitutionId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(200);

        builder.Property(a => a.Currency)
            .HasMaxLength(3);

        builder.Property(a => a.AccountType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Create index on Status for filtering active accounts
        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Accounts_Status");
    }
}
