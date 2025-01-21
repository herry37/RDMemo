using BackendManagement.Infrastructure.Monitoring.Metrics;

namespace BackendManagement.Infrastructure.Monitoring.Middleware;

/// <summary>
/// 指標收集中介軟體
/// </summary>
public class MetricsMiddleware
{
    private readonly RequestDelegate _next;

    public MetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        var method = context.Request.Method;

        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);

            MetricsRegistry.IncrementHttpRequests(method, path!, context.Response.StatusCode.ToString());
        }
        finally
        {
            sw.Stop();
            MetricsRegistry.ObserveHttpDuration(method, path!, sw.Elapsed.TotalSeconds);
        }
    }
} 