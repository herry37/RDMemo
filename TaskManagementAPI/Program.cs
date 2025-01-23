/// <summary>
/// 待辦事項管理系統的主程序入口點
/// 負責配置和啟動 Web API 服務
/// </summary>

using Microsoft.EntityFrameworkCore;
using TodoTaskManagementAPI.Infrastructure;
using TodoTaskManagementAPI.Services;

/// <summary>
/// 設置應用程序環境為生產環境
/// 這確保應用程序在所有環境中使用相同的配置
/// </summary>
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

/// <summary>
/// 創建 Web 應用程序構建器
/// 設置基本配置選項包括環境、根目錄等
/// </summary>
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = "Production",  // 設置為生產環境
    WebRootPath = "wwwroot",        // 設置靜態文件根目錄
    ContentRootPath = Directory.GetCurrentDirectory() // 設置應用程序根目錄
});

/// <summary>
/// 服務配置區域
/// 配置應用程序所需的各種服務
/// </summary>

// 添加 MVC 控制器服務
builder.Services.AddControllers();

/// <summary>
/// CORS 配置
/// 允許跨域資源共享，使前端能夠訪問 API
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()     // 允許任何來源
              .AllowAnyMethod()     // 允許任何 HTTP 方法
              .AllowAnyHeader();    // 允許任何標頭
    });
});

/// <summary>
/// 數據庫配置
/// 使用 SQLite 作為數據存儲，配置數據庫連接
/// </summary>
builder.Services.AddDbContext<TodoTaskContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("找不到數據庫連接字符串 'DefaultConnection'");
    }
    options.UseSqlite(connectionString);
});

/// <summary>
/// 依賴注入配置
/// 注冊服務和倉儲的實現
/// </summary>
builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();        // 注入任務服務
builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();  // 注入任務倉儲

/// <summary>
/// 創建 Web 應用程序實例
/// </summary>
var app = builder.Build();

/// <summary>
/// HTTP 請求管道配置
/// 配置中間件處理 HTTP 請求
/// </summary>

// 配置靜態文件服務
app.UseDefaultFiles();  // 允許默認文件（如 index.html）
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 設置緩存控制標頭，防止瀏覽器緩存靜態文件
        ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
        ctx.Context.Response.Headers["Pragma"] = "no-cache";
        ctx.Context.Response.Headers["Expires"] = "-1";
    }
});

/// <summary>
/// 啟用 CORS 中間件
/// 必須在路由之前配置
/// </summary>
app.UseCors();

/// <summary>
/// 路由配置
/// 配置 API 端點和回退路由
/// </summary>
app.MapControllers();  // 映射 API 控制器路由

// 配置 SPA 回退路由
app.MapFallbackToFile("index.html");  // 將未匹配的請求重定向到 index.html

/// <summary>
/// 數據庫初始化
/// 確保數據庫存在並已創建所需的表
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoTaskContext>();
    context.Database.EnsureCreated();
}

/// <summary>
/// 啟動應用程序
/// 開始監聽 HTTP 請求
/// </summary>
app.Run();
