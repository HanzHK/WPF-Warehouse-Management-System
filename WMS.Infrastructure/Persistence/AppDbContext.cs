using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the application.
/// Holds DbSets and configures the database model.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Business partners (suppliers, recipients, customers…).
    /// </summary>
    public DbSet<BusinessPartner> BusinessPartners { get; set; }

    /// <summary>
    /// Physical addresses.
    /// </summary>
    public DbSet<Address> Addresses { get; set; }

    /// <summary>
    /// Countries used for addressing.
    /// </summary>
    public DbSet<Country> Countries { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
