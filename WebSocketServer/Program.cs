using WebSocketServer.Middleware;
using WebSocketServer.Services;

var builder = WebApplication.CreateBuilder(args);

// 配置 HttpClient
builder.Services.AddHttpClient<ITruckLocationService, TruckLocationService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "WebSocketServer");//設定預設標頭
    client.Timeout = TimeSpan.FromSeconds(30);//設定超時時間
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
});

// 配置 CORS
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

// 配置日誌記錄（只記錄警告和錯誤）
builder.Logging.ClearProviders();
builder.Logging.AddConsole()
    .SetMinimumLevel(LogLevel.Warning);

var app = builder.Build();

// 配置靜態檔案
app.UseDefaultFiles();
app.UseStaticFiles();

// 配置中間件
app.UseRouting();
app.UseCors();

// 配置 WebSocket
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
};
app.UseWebSockets(webSocketOptions);
app.UseWebSocketHandler();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"應用程式啟動失敗：{ex.Message}");
    throw;
}
