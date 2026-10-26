using CircuitBreakerTestApp.Application.Abstractions;
using CircuitBreakerTestApp.Infrastructure.Resilience;
using Polly;
using Polly.Registry;

namespace CircuitBreakerTestApp.Infrastructure.Persistence;

public sealed class InventoryRepository(ResiliencePipelineProvider<string> provider) : IInventoryRepository
{
    private readonly ResiliencePipeline _pipeline = provider.GetPipeline(PipelineNames.Database);
    public async Task<string> GetItemNameAsync(int id, CancellationToken ct) =>
    
        await _pipeline.ExecuteAsync(async token =>
        {
            await Task.Delay(10, token); // replace with real DB call
            return $"item-{id}";
        }, ct);
    
}