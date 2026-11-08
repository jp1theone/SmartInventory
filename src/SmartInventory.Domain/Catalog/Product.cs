using SmartInventory.Domain.Common;

namespace SmartInventory.Domain.Catalog;

public sealed class Product
{
    private Product() { }

    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static Product Create(string sku, string name, string category, decimal unitPrice, DateTimeOffset now)
    {
        sku = sku.Trim().ToUpperInvariant();
        name = name.Trim();
        category = category.Trim();

        if (sku.Length is < 2 or > 32) throw new DomainException("El SKU debe tener entre 2 y 32 caracteres.");
        if (name.Length is < 2 or > 120) throw new DomainException("El nombre debe tener entre 2 y 120 caracteres.");
        if (category.Length is < 2 or > 80) throw new DomainException("La categoría debe tener entre 2 y 80 caracteres.");
        if (unitPrice < 0) throw new DomainException("El precio no puede ser negativo.");

        return new Product
        {
            Id = Guid.NewGuid(), Sku = sku, Name = name, Category = category,
            UnitPrice = decimal.Round(unitPrice, 2), IsActive = true, CreatedAtUtc = now
        };
    }
}
