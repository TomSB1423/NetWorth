using Microsoft.EntityFrameworkCore;
using Networth.Domain.Entities;

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
    /// </summary>
    public DbSet<Institution> Institutions { get; set; } = null!;

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

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetworthDbContext).Assembly);
    }
}
