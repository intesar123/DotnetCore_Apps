using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Polly.CircuitBreaker;

namespace CircuitBreakerTestApp.Common.Errors;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            BrokenCircuitException => (StatusCodes.Status503ServiceUnavailable, "Dependency temporarily unavailable"),
            HttpRequestException => (StatusCodes.Status502BadGateway, "Upstream Service Error"),
            TimeoutException or TaskCanceledException when !httpContext.RequestAborted.IsCancellationRequested => (StatusCodes.Status504GatewayTimeout, "Upstream Timeout"),
            _ => (0, string.Empty)
        };

        if (status == 0) return false; // fall through to default handling

        logger.LogWarning(exception, "Handled dependency failure: {Title}", title);

        httpContext.Response.StatusCode = status;

        if (status == StatusCodes.Status503ServiceUnavailable)
            httpContext.Response.Headers.RetryAfter = "15";

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = title }, ct);

        return true;
    }
}