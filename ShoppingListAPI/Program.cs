using Microsoft.OpenApi.Models;
using ShoppingListAPI.Services.FileDb;
using ShoppingListAPI.Services.WebSocket;
using ShoppingListAPI.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShoppingListAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 配置日誌
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
        {
            options.FormatterName = "simple";
        });

        // 關閉 Microsoft 和 System 命名空間的除錯訊息
        builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

        builder.Logging.AddFilter("System", LogLevel.Warning);

        builder.Logging.AddFilter("Microsoft.AspNetCore.Watch.BrowserRefresh.*", LogLevel.None);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.None);
        builder.Logging.AddFilter("Microsoft.AspNetCore.StaticFiles.*", LogLevel.None);
        builder.Logging.AddFilter("Microsoft.AspNetCore.SpaServices.*", LogLevel.None);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Mvc.StaticFiles.*", LogLevel.None);

        // 確保 Data\\FileStore\\shoppinglists 目錄存在
        var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data", "FileStore", "shoppinglists");
        Directory.CreateDirectory(dataDirectory); // 確保目錄存在

        // 配置資料目錄
        builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
        // builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            {"DataDirectory", Path.Combine(builder.Environment.ContentRootPath, "Data", "FileStore", "shoppinglists")}
        });

        // 配置服務
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shopping List API", Version = "v1" });
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
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        // 關閉開發時期的瀏覽器連結和自動重新載入
        if (builder.Environment.IsDevelopment())
        {
            // 關閉 Browser Link
            builder.WebHost.UseSetting("DetailedErrors", "false");
            builder.WebHost.UseSetting("SuppressStatusMessages", "true");
            builder.WebHost.UseSetting("Microsoft.AspNetCore.Watch.BrowserRefresh.Enabled", "false");
            builder.WebHost.UseSetting("Microsoft.AspNetCore.Watch.UsePollingFileWatcher", "false");
            builder.WebHost.UseSetting("Microsoft.AspNetCore.StaticFiles.BrowserLink.Enabled", "false");
        }

        var app = builder.Build();

        // 配置中介軟體順序
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // 啟用 HTTPS
        app.UseHttpsRedirection();

        // 啟用預設檔案
        app.UseDefaultFiles(new DefaultFilesOptions
        {
            DefaultFileNames = new List<string> { "index.html" }
        });
        // 啟用靜態檔案
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                // 設置快取標頭
                ctx.Context.Response.Headers.Append(
                    "Cache-Control", $"public, max-age=31536000");
            }
        });

        // 啟用CORS
        app.UseCors();

        // 啟用 WebSocket
        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    try
                    {
                        var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
                        await webSocketHandler.HandleWebSocketConnection(context);
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync("WebSocket 連接失敗");
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("需要 WebSocket 請求");
                }
            }
            else
            {
                await next();
            }
        });

        // 啟用路由
        app.UseRouting();

        // 啟用路由端點
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });

        app.Run();
    }
}
