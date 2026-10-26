using CircuitBreakerTestApp.Common.Errors;
using CircuitBreakerTestApp.Infrastructure.Resilience;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddResilienceSettings(builder.Configuration);
builder.Services.AddOrderHttpClient(builder.Configuration);
builder.Services.AddDatabaseResiliencePipeline();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
