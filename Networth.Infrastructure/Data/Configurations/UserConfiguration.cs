using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Networth.Infrastructure.Data.Entities;

namespace Networth.Infrastructure.Data.Configurations;

/// <summary>
///     Entity Framework configuration for the User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Name)
            .HasMaxLength(200)
            .IsRequired();

        // Relationships
        builder.HasMany(u => u.Accounts)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Transactions)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
