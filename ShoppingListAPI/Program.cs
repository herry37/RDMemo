using ShoppingListAPI.Models;
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

        // 確保 Data\\FileStore\\shoppinglists 目錄存在
        var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data", "FileStore", "shoppinglists");
        Directory.CreateDirectory(dataDirectory); // 確保目錄存在

        // 確保購物清單檔案存在
        var shoppingListsFile = Path.Combine(dataDirectory, "shopping_lists.json");
        if (!File.Exists(shoppingListsFile))
        {
            // 建立測試資料
            var testList = new ShoppingList
            {
                Id = Guid.NewGuid().ToString(),
                Title = "測試購物清單",
                BuyDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                totalAmount = 1000,
                Items = new List<ShoppingItem>
                {
                    new ShoppingItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "測試商品",
                        Price = 1000,
                        Quantity = 1
                    }
                }
            };

            var json = JsonSerializer.Serialize(
                new List<ShoppingList> { testList },
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            );
            File.WriteAllText(shoppingListsFile, json);
        }

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
