using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using ShoppingListAPI.Services.FileDb;
using ShoppingListAPI.Services.WebSocket;
using ShoppingListAPI.Utils;
using System.Net.WebSockets;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 添加服務
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 添加速率限制
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueLimit = 2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

// 註冊服務
builder.Services.AddSingleton<IFileDbService, FileDbService>();
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddScoped<DataGenerator>();
builder.Services.AddMemoryCache();

// 添加CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(_ => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// 啟用速率限制
app.UseRateLimiter();

// 啟用CORS
app.UseCors();

// 配置 WebSocket
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

// WebSocket 端點
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        await handler.HandleWebSocketConnection(context);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

// 配置靜態檔案
var staticFileOptions = new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream"
};

// 啟用預設檔案（如 index.html）
app.UseDefaultFiles();

// 啟用靜態檔案服務
app.UseStaticFiles(staticFileOptions);

// API路由
app.MapControllers();

// 將所有未匹配的路由導向index.html
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    // 暫時註解掉測試資料生成
    /*
    // 生成測試資料
    using var scope = app.Services.CreateScope();
    var dataGenerator = scope.ServiceProvider.GetRequiredService<DataGenerator>();
    try
    {
        await dataGenerator.GenerateTestDataAsync(200);
        Console.WriteLine("成功生成200筆測試資料");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "生成測試資料時發生錯誤");
    }
    */
}

Console.WriteLine("\n應用程式已啟動:");
Console.WriteLine("前端網站: http://localhost:5001");
Console.WriteLine("API位址: http://localhost:5001/api/ShoppingList");
Console.WriteLine("WebSocket位址: ws://localhost:5001/ws");

app.Run();
