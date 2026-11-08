namespace SmartInventory.Application.Common;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
