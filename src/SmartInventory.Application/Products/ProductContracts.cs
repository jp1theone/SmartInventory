using SmartInventory.Domain.Catalog;

namespace SmartInventory.Application.Products;

public sealed record ProductDto(Guid Id, string Sku, string Name, string Category, decimal UnitPrice, bool IsActive, DateTimeOffset CreatedAtUtc);
public sealed record CreateProductRequest(string Sku, string Name, string Category, decimal UnitPrice);

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);
    Task<bool> SkuExistsAsync(string normalizedSku, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
}

public sealed class ProductCatalogService(IProductRepository repository, Common.IClock clock)
{
    public async Task<IReadOnlyList<ProductDto>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var products = await repository.SearchAsync(search?.Trim(), page, pageSize, cancellationToken);
        return products.Select(ToDto).ToArray();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var sku = request.Sku.Trim().ToUpperInvariant();
        if (await repository.SkuExistsAsync(sku, cancellationToken))
            throw new InvalidOperationException($"Ya existe un producto con el SKU '{sku}'.");

        var product = Product.Create(request.Sku, request.Name, request.Category, request.UnitPrice, clock.UtcNow);
        await repository.AddAsync(product, cancellationToken);
        return ToDto(product);
    }

    private static ProductDto ToDto(Product product) => new(
        product.Id, product.Sku, product.Name, product.Category, product.UnitPrice, product.IsActive, product.CreatedAtUtc);
}
