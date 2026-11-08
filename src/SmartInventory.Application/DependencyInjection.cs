using Microsoft.Extensions.DependencyInjection;
using SmartInventory.Application.Products;

namespace SmartInventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductCatalogService>();
        return services;
    }
}
