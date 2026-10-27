using CircuitBreakerTestApp.Application.Abstractions;
using CircuitBreakerTestApp.Application.Services;
using CircuitBreakerTestApp.Common.Errors;
using CircuitBreakerTestApp.Infrastructure.Persistence;
using CircuitBreakerTestApp.Infrastructure.Resilience;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddResilienceSettings(builder.Configuration);
builder.Services.AddOrderHttpClient(builder.Configuration);
builder.Services.AddDatabaseResiliencePipeline();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Maps the interactive Scalar UI endpoint
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Circuit Breaker test app")
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
