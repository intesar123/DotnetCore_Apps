using CircuitBreakerTestApp.Application.Services;
using CircuitBreakerTestApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CircuitBreakerDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrderController(IOrderService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType<Order>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var order = await service.GetOrderAsync(id,ct);
        return order is null? NotFound() : Ok(order);
    }
}