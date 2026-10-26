namespace CircuitBreakerTestApp.Application.Abstractions;
using CircuitBreakerTestApp.Domain;
using System.Threading;
using System.Threading.Tasks;
public interface IOrdersClient
{
    Task<Order?> GetOrderAsync(int id, CancellationToken cancellationToken);
}

