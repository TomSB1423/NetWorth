using Microsoft.EntityFrameworkCore;
using Networth.Infrastructure.Data.Entities;
using Account = Networth.Infrastructure.Data.Entities.Account;
using Transaction = Networth.Infrastructure.Data.Entities.Transaction;

namespace Networth.Infrastructure.Data.Context;

/// <summary>
///     Entity Framework DbContext for the Networth application.
/// </summary>
public class NetworthDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NetworthDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public NetworthDbContext(DbContextOptions<NetworthDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    ///     Gets or sets the Users DbSet.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the Institutions DbSet.
    ///     For caching institution information from GoCardless.
    /// </summary>
    public DbSet<InstitutionMetadata> Institutions { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the UserInstitutions DbSet.
    ///     For tracking user connections to institutions.
    /// </summary>
    public DbSet<Institution> UserInstitutions { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the Accounts DbSet.
    /// </summary>
    public DbSet<Account> Accounts { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the Transactions DbSet.
    /// </summary>
    public DbSet<Transaction> Transactions { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the Requisitions DbSet.
    ///     For tracking GoCardless requisition state.
    /// </summary>
    public DbSet<Requisition> Requisitions { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the CacheMetadata DbSet.
    ///     For tracking data cache freshness.
    /// </summary>
    public DbSet<CacheMetadata> CacheMetadata { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetworthDbContext).Assembly);

        // Seed mock user for development/testing
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = "mock-user-123", Name = "Mock Development User",
        });
    }
}
