using CircuitBreakerTestApp.Application.Abstractions;
using CircuitBreakerTestApp.Domain;

namespace CircuitBreakerTestApp.Infrastructure.Http;

public class OrdersClient(HttpClient http) : IOrdersClient
{
    public async Task<Order?> GetOrderAsync(int id, CancellationToken ct)
    {
        using var resp = await http.GetAsync($"/orders/{id}", ct);
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<Order>(ct);
    }
}