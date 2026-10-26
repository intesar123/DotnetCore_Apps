using CircuitBreakerTestApp.Application.Abstractions;
using CircuitBreakerTestApp.Infrastructure.Http;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;

namespace CircuitBreakerTestApp.Infrastructure.Resilience;

public static class ResilienceServiceCollectionExtensions
{
    public static IServiceCollection AddResilienceSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ResilienceSettings>(config.GetSection(ResilienceSettings.SectionName));
        return services;
    }
    
    public static IServiceCollection AddOrderHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<IOrdersClient, OrdersClient>(c=>
        c.BaseAddress = new Uri(config["OrdersApi:BaseUrl"] ?? throw new InvalidOperationException("OrdersApi:BaseUrl is missing")))
        .AddResilienceHandler(PipelineNames.Orders, (pipeline,ctx) =>
        {
            var s= ctx.ServiceProvider.GetRequiredService<IOptions<ResilienceSettings>>().Value.Orders;
            var logger = ctx.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Resilience.Orders");

            pipeline.AddTimeout(TimeSpan.FromSeconds(s.TotalTimeoutSeconds));

            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = s.MaxRetryAttempts,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(200),
                UseJitter = true
            });

            pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = s.FailureRatio,
                MinimumThroughput = s.MinimumThroughput,
                SamplingDuration = TimeSpan.FromSeconds(s.SamplingDurationSeconds),
                BreakDuration = TimeSpan.FromSeconds(s.BreakDurationSeconds),
                OnOpened = a =>
                {
                    logger.LogWarning("Orders circuit OPENED for {Duration}",a.BreakDuration);
                    return default;
                },
                OnClosed = _ =>
                {
                    logger.LogInformation("Orders circuit CLOSED");
                    return default;
                },
                OnHalfOpened = _ =>
                {
                    logger.LogInformation("Orders circuit HALF-OPEN");
                    return default;
                }
            });

            pipeline.AddTimeout(TimeSpan.FromSeconds(s.AttemptTimeoutSeconds));

        });

        return services;
    }

    public static IServiceCollection AddDatabaseResiliencePipeline(this IServiceCollection services)
    {
        services.AddResiliencePipeline(PipelineNames.Database, (pipeline,ctx)=>
        {
            var s = ctx.ServiceProvider.GetRequiredService<IOptions<ResilienceSettings>>().Value.Database;
            var logger = ctx.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Resilience.Database");

            pipeline.AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = s.FailureRatio,
                MinimumThroughput = s.MinimumThroughput,
                SamplingDuration = TimeSpan.FromSeconds(s.SamplingDurationSeconds),
                BreakDuration = TimeSpan.FromSeconds(s.BreakDurationSeconds),
                // Replace/add provider-specific exceptions (e.g SqlExceptions)
                ShouldHandle = new PredicateBuilder()
                .Handle<TimeoutException>()
                .Handle<InvalidOperationException>(),
                OnOpened = a =>
                {
                    logger.LogWarning("DB circuit OPENED for {Duration}",a.BreakDuration);
                    return default;
                }
            });

        });

        return services;
    }

}