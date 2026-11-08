using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartInventory.Application.Common;
using SmartInventory.Application.Products;
using SmartInventory.Infrastructure.Persistence;

namespace SmartInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmartInventory")
            ?? throw new InvalidOperationException("Configura ConnectionStrings:SmartInventory mediante User Secrets o variables de entorno.");

        services.AddDbContext<SmartInventoryDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
