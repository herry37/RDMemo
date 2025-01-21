namespace BackendManagement.Infrastructure.MultiTenancy;

/// <summary>
/// 租戶中介軟體
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(
        RequestDelegate next,
        ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantService tenantService)
    {
        var tenant = await tenantService.GetTenantAsync();
        if (tenant == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "無效的租戶ID"
            });
            return;
        }

        // 將租戶資訊加入HttpContext
        context.Items["Tenant"] = tenant;

        await _next(context);
    }
} 