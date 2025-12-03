using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Configures the <see cref="Entities.AccountBalance" /> entity.
/// </summary>
public class AccountBalanceConfiguration : IEntityTypeConfiguration<Entities.AccountBalance>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Entities.AccountBalance> builder)
    {
        builder.ToTable("AccountBalances");

        builder.HasKey(ab => ab.Id);

        builder.Property(ab => ab.Id)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ab => ab.AccountId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ab => ab.BalanceType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ab => ab.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(ab => ab.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(ab => ab.RetrievedAt)
            .IsRequired();

        // Foreign Keys
        builder.HasOne<Entities.Account>()
            .WithMany(a => a.Balances)
            .HasForeignKey(ab => ab.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ab => ab.AccountId);
        builder.HasIndex(ab => ab.RetrievedAt);
        builder.HasIndex(ab => new { ab.AccountId, ab.BalanceType, ab.RetrievedAt });
    }
}
