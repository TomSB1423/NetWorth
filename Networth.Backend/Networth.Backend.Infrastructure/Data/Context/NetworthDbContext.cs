using Microsoft.EntityFrameworkCore;
using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Infrastructure.Data.Context;

/// <summary>
///     Entity Framework DbContext for the Networth application.
///     Simplified to only include entities that need local storage.
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
    ///     Gets or sets the Requisitions DbSet.
    ///     Essential for tracking account linking workflow.
    /// </summary>
    public DbSet<Requisition> Requisitions { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the Accounts DbSet.
    ///     For caching account data locally if needed.
    /// </summary>
    public DbSet<Account> Accounts { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetworthDbContext).Assembly);
    }
}
