using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using ShoppingListAPI.Services.FileDb;
using ShoppingListAPI.Services.WebSocket;
using ShoppingListAPI.Utils;
using System.Net.WebSockets;
using System.Threading.RateLimiting;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 添加服務
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    });
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

// 確保資料目錄存在
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data", "FileStore", "shoppinglists");
Directory.CreateDirectory(dataDirectory);
Console.WriteLine($"資料目錄：{dataDirectory}");

// 在開發環境中生成測試資料
if (app.Environment.IsDevelopment())
{
    try 
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var fileDbService = services.GetRequiredService<IFileDbService>();
        
        // 檢查是否已有資料
        logger.LogInformation("開始檢查現有資料");
        var existingLists = await fileDbService.GetAllAsync();
        logger.LogInformation($"找到 {existingLists.Count} 筆現有的購物清單");
        
        if (existingLists.Count == 0)
        {
            logger.LogInformation("沒有找到現有的購物清單，開始生成測試資料");
            var dataGenerator = services.GetRequiredService<DataGenerator>();
            await dataGenerator.GenerateTestDataAsync(10);
            logger.LogInformation("測試資料生成完成");

            // 再次檢查資料
            existingLists = await fileDbService.GetAllAsync();
            logger.LogInformation($"重新檢查：現在有 {existingLists.Count} 筆購物清單");
        }
        else
        {
            logger.LogInformation($"已找到 {existingLists.Count} 筆現有的購物清單，跳過測試資料生成");
        }
    }
    catch (Exception ex)
    {
        var services = app.Services.GetService<ILogger<Program>>();
        services?.LogError(ex, "生成測試資料時發生錯誤");
    }
}

// 配置中介軟體順序
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 啟用CORS - 必須在其他中介軟體之前
app.UseCors();

// 啟用速率限制
app.UseRateLimiter();

// 配置 WebSocket
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

// 確保在 UseRouting 之後，但在 UseEndpoints 之前配置 WebSocket
app.UseRouting();
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
app.UseDefaultFiles();
app.UseStaticFiles();

// API路由
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("index.html");
});

Console.WriteLine("\n應用程式已啟動:");
Console.WriteLine($"前端網站: http://localhost:{builder.Configuration["Ports:Http"] ?? "5002"}");
Console.WriteLine($"API位址: http://localhost:{builder.Configuration["Ports:Http"] ?? "5002"}/api/shoppinglists");
Console.WriteLine($"WebSocket位址: ws://localhost:{builder.Configuration["Ports:Http"] ?? "5002"}/ws");

app.Run();
