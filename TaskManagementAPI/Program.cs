using Microsoft.EntityFrameworkCore;
using TodoTaskManagementAPI.Infrastructure;
using TodoTaskManagementAPI.Services;

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = "Production",
    WebRootPath = "wwwroot",
    ContentRootPath = Directory.GetCurrentDirectory()
});

// Add services to the container.
builder.Services.AddControllers();

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 配置數據庫
builder.Services.AddDbContext<TodoTaskContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Database connection string 'DefaultConnection' not found in configuration.");
    }
    options.UseSqlite(connectionString);
});

// 注入服務
builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();
builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
        ctx.Context.Response.Headers["Pragma"] = "no-cache";
        ctx.Context.Response.Headers["Expires"] = "-1";
    }
});

// 使用 CORS
app.UseCors();

// API 端點
app.MapControllers();

// 添加一個回退路由，將所有不匹配的請求重定向到 index.html
app.MapFallbackToFile("index.html");

// 確保數據庫已創建
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoTaskContext>();
    context.Database.EnsureCreated();
}

app.Run();
