using CircuitBreakerTestApp.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CircuitBreakerDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InventoryController(IInventoryRepository repo) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<string>> Get(int id, CancellationToken ct) =>
        Ok(await repo.GetItemNameAsync(id, ct));
}