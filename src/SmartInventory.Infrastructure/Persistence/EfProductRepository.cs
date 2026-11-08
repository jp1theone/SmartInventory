using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Products;
using SmartInventory.Domain.Catalog;

namespace SmartInventory.Infrastructure.Persistence;

internal sealed class EfProductRepository(SmartInventoryDbContext dbContext) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsNoTracking().Where(product => product.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product => product.Sku.Contains(term) || product.Name.Contains(term) || product.Category.Contains(term));
        }

        return await query.OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> SkuExistsAsync(string normalizedSku, CancellationToken cancellationToken) =>
        dbContext.Products.AnyAsync(product => product.Sku == normalizedSku, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
