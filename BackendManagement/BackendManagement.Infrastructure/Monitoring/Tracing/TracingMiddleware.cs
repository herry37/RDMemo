namespace BackendManagement.Infrastructure.Monitoring.Tracing;

/// <summary>
/// 追蹤中介軟體
/// </summary>
public class TracingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TracingMiddleware> _logger;
    private readonly ActivitySource _activitySource;

    public TracingMiddleware(
        RequestDelegate next,
        ILogger<TracingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _activitySource = new ActivitySource("BackendManagement");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var activity = _activitySource.StartActivity(
            $"{context.Request.Method} {context.Request.Path}",
            ActivityKind.Server);

        try
        {
            // 加入基本資訊
            activity?.SetTag("http.method", context.Request.Method);
            activity?.SetTag("http.url", context.Request.Path);
            activity?.SetTag("http.host", context.Request.Host.ToString());

            // 加入請求標頭
            foreach (var header in context.Request.Headers)
            {
                activity?.SetTag($"http.header.{header.Key.ToLower()}", header.Value);
            }

            await _next(context);

            // 加入回應資訊
            activity?.SetTag("http.status_code", context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("error.stack_trace", ex.StackTrace);
            _logger.LogError(ex, "請求處理失敗");
            throw;
        }
    }
} 