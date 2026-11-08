using Microsoft.EntityFrameworkCore;
using SmartInventory.Domain.Catalog;

namespace SmartInventory.Infrastructure.Persistence;

public sealed class SmartInventoryDbContext(DbContextOptions<SmartInventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Sku).HasMaxLength(32).IsRequired();
            entity.HasIndex(product => product.Sku).IsUnique();
            entity.Property(product => product.Name).HasMaxLength(120).IsRequired();
            entity.Property(product => product.Category).HasMaxLength(80).IsRequired();
            entity.Property(product => product.UnitPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouses");
            entity.HasKey(warehouse => warehouse.Id);
            entity.Property(warehouse => warehouse.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(warehouse => warehouse.Code).IsUnique();
            entity.Property(warehouse => warehouse.Name).HasMaxLength(120).IsRequired();
        });
    }
}
