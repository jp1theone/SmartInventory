using SmartInventory.Domain.Common;

namespace SmartInventory.Domain.Catalog;

public sealed class Warehouse
{
    private Warehouse() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static Warehouse Create(string code, string name, DateTimeOffset now)
    {
        code = code.Trim().ToUpperInvariant();
        name = name.Trim();
        if (code.Length is < 2 or > 20) throw new DomainException("El código debe tener entre 2 y 20 caracteres.");
        if (name.Length is < 2 or > 120) throw new DomainException("El nombre debe tener entre 2 y 120 caracteres.");
        return new Warehouse { Id = Guid.NewGuid(), Code = code, Name = name, IsActive = true, CreatedAtUtc = now };
    }
}
