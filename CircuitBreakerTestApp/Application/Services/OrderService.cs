using CircuitBreakerTestApp.Application.Abstractions;
using CircuitBreakerTestApp.Domain;

namespace CircuitBreakerTestApp.Application.Services;

public interface IOrderService
{
    Task<Order?> GetOrderAsync(int id, CancellationToken ct);
}

public sealed class OrderService(IOrdersClient client) : IOrderService
{
    public Task<Order?> GetOrderAsync(int id, CancellationToken ct)=> client.GetOrderAsync(id,ct);
}