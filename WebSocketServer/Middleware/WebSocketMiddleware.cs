using WebSocketServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebSocketServer.Middleware;

/// <summary>
/// WebSocket 中間件
/// 負責處理 WebSocket 連線請求並管理資料傳輸
/// </summary>
public class WebSocketMiddleware
{
    // 下一個中間件委派
    private readonly RequestDelegate _next;
    // 服務範圍工廠，用於建立服務範圍
    private readonly IServiceScopeFactory _serviceScopeFactory;
    // 日誌記錄器
    private readonly ILogger<WebSocketMiddleware> _logger;

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="next">下一個中間件委派</param>
    /// <param name="serviceScopeFactory">服務範圍工廠</param>
    /// <param name="logger">日誌記錄器</param>
    public WebSocketMiddleware(
        RequestDelegate next,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<WebSocketMiddleware> logger)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// 處理 HTTP 請求的非同步方法
    /// </summary>
    /// <param name="context">HTTP 內容</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // 檢查是否為 WebSocket 請求路徑
        if (context.Request.Path == "/ws")
        {
            // 檢查是否為有效的 WebSocket 請求
            if (context.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    // 接受 WebSocket 連線
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    // 建立新的服務範圍
                    using var scope = _serviceScopeFactory.CreateScope();
                    // 從服務範圍中取得垃圾車位置服務
                    var truckLocationService = scope.ServiceProvider.GetRequiredService<ITruckLocationService>();
                    
                    // 建立取消權杖來處理連線關閉
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
                    // 開始傳送垃圾車位置資料
                    await truckLocationService.SendTruckLocationsAsync(webSocket, cts.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "WebSocket 連線發生錯誤");
                }
            }
            else
            {
                _logger.LogWarning("收到非 WebSocket 請求到 /ws 端點");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            // 如果不是 WebSocket 請求，則傳遞給下一個中間件
            await _next(context);
        }
    }
}

/// <summary>
/// WebSocket 中間件的擴充方法
/// </summary>
public static class WebSocketMiddlewareExtensions
{
    /// <summary>
    /// 將 WebSocket 處理中間件加入到應用程式管線中
    /// </summary>
    /// <param name="app">應用程式建構器</param>
    /// <returns>應用程式建構器</returns>
    public static IApplicationBuilder UseWebSocketHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<WebSocketMiddleware>();
    }
}
