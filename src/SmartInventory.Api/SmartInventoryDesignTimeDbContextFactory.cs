using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SmartInventory.Infrastructure.Persistence;

public sealed class SmartInventoryDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SmartInventoryDbContext>
{
    public SmartInventoryDbContext CreateDbContext(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(root)
            .AddJsonFile("src/SmartInventory.Api/appsettings.json", optional: true)
            .AddJsonFile("src/SmartInventory.Api/appsettings.Development.json", optional: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("SmartInventory")
            ?? throw new InvalidOperationException("Define ConnectionStrings:SmartInventory en User Secrets o en una variable de entorno antes de aplicar migraciones.");
        var options = new DbContextOptionsBuilder<SmartInventoryDbContext>().UseSqlServer(connectionString).Options;
        return new SmartInventoryDbContext(options);
    }
}
