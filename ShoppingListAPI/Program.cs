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
using Microsoft.Extensions.Logging;
using System.IO;

namespace ShoppingListAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 配置端口
        builder.WebHost.UseUrls("http://localhost:5002");

        // 配置日誌
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        // 配置資料目錄
        builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            {"DataDirectory", Path.Combine(builder.Environment.ContentRootPath, "Data", "FileStore", "shoppinglists")}
        });

        // 在開發環境中啟用詳細日誌
        if (builder.Environment.IsDevelopment())
        {
            builder.Logging.AddDebug();
            builder.Logging.AddConsole(options =>
            {
                options.IncludeScopes = true;
            });
        }

        // 添加服務
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        var app = builder.Build();

        // 配置中介軟體順序
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        // 啟用 HTTPS
        app.UseHttpsRedirection();

        // 啟用靜態檔案
        app.UseDefaultFiles(new DefaultFilesOptions
        {
            DefaultFileNames = new List<string> { "index.html" }
        });
        app.UseStaticFiles();

        // 啟用CORS
        app.UseCors();

        // 啟用 WebSocket
        var webSocketOptions = new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        };
        app.UseWebSockets(webSocketOptions);

        // WebSocket 路由處理
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
                    await webSocketHandler.HandleWebSocketConnection(context);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
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
