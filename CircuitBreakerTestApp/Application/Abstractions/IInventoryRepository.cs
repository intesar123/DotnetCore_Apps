namespace CircuitBreakerTestApp.Application.Abstractions;

public interface IInventoryRepository
{
    Task<string> GetItemNameAsync(int id, CancellationToken ct);
}