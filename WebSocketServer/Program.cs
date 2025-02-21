using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using WebSocketServer.Services;

/// <summary>
///  Program 的主要進入點
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// 配置記憶體快取
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
    options.CompactionPercentage = 0.25;
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
});

/// <summary>
///  配置日誌系統
/// </summary>
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

/// <summary>
///  Add services to the container
/// </summary>
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });

builder.Services.AddScoped<ITruckLocationService, TruckLocationService>();

/// <summary>
///  Configure HttpClient
/// </summary>
builder.Services.AddHttpClient("TruckLocation", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    // 設定預設標頭
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
    UseProxy = false,
    AllowAutoRedirect = true
});

/// <summary>
///  Configure Kestrel
/// </summary>
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxConcurrentConnections = 100;
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
    serverOptions.AllowSynchronousIO = true;  // 允許同步 IO
});

/// <summary>
///  Configure Response Compression
/// </summary>
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

/// <summary>
///  Configure CORS
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(origin =>
        {
            var host = new Uri(origin).Host;
            return host.Equals("localhost") ||
                   host.Equals("127.0.0.1") ||
                   host.EndsWith(".somee.com") ||
                   host.EndsWith(".kcg.gov.tw");
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Content-Disposition");
    });
});

/// <summary>
///  Configure Health Checks
/// </summary>
builder.Services.AddHealthChecks()
    .AddMemoryHealthCheck("memory");
// .AddUrlGroup(new Uri(builder.Configuration["ApiSettings:LocalBaseUrl"]), "external_api");

var app = builder.Build();

/// <summary>
///  Configure the HTTP request pipeline
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

/// <summary>
///  Enable Response Compression
/// </summary>
app.UseResponseCompression();

// 啟用 CORS - 移到最前面的中介軟體
app.UseCors(builder =>
{
    builder.SetIsOriginAllowed(origin =>
    {
        var host = new Uri(origin).Host;
        return host.Equals("localhost") ||
               host.Equals("127.0.0.1") ||
               host.EndsWith(".somee.com") ||
               host.EndsWith(".kcg.gov.tw");
    })
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithExposedHeaders("Content-Disposition");
});

// 配置路由中介軟體
app.Use(async (context, next) =>
{
    try
    {
        var originalPath = context.Request.Path.Value ?? string.Empty;
        app.Logger.LogInformation("收到請求: {Path}", originalPath);

        // 處理 API 請求
        if (originalPath.StartsWith("/WebSocketServer/api/", StringComparison.OrdinalIgnoreCase))
        {
            var newPath = originalPath.Replace("/WebSocketServer/api/", "/api/");
            app.Logger.LogInformation("API 路徑重寫: {OldPath} => {NewPath}", originalPath, newPath);
            context.Request.Path = newPath;
        }
        // 處理靜態檔案請求
        else if (originalPath.StartsWith("/WebSocketServer/", StringComparison.OrdinalIgnoreCase))
        {
            var newPath = originalPath.Replace("/WebSocketServer/", "/");
            app.Logger.LogInformation("靜態檔案路徑重寫: {OldPath} => {NewPath}", originalPath, newPath);
            context.Request.Path = newPath;
        }

        await next();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "請求處理過程中發生錯誤");
        throw;
    }
});

// 配置 API 路由
app.MapControllers();

// 配置靜態檔案
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
            "Cache-Control", $"public, max-age=3600");
    }
});

// 配置 CSP - 放在靜態檔案之後
app.Use(async (context, next) =>
{
    context.Response.Headers.Add(
        "Content-Security-Policy",
        "default-src 'self' https://api.kcg.gov.tw; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://unpkg.com https://cdnjs.cloudflare.com; " +
        "style-src 'self' 'unsafe-inline' https://unpkg.com https://cdnjs.cloudflare.com; " +
        "img-src 'self' data: https://*.openstreetmap.org https://*.tile.openstreetmap.org https://unpkg.com https://cdnjs.cloudflare.com blob: *; " +
        "font-src 'self' data: https://cdnjs.cloudflare.com; " +
        "connect-src 'self' https://api.kcg.gov.tw https://*.openstreetmap.org https://*.tile.openstreetmap.org;"
    );

    await next();
});

/// <summary>
///  Configure Health Check Endpoint
/// </summary>
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(e => new
            {
                Component = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description
            }),
            TotalDuration = report.TotalDuration
        };
        await JsonSerializer.SerializeAsync(context.Response.Body, response);
    }
});

app.Run();

/// <summary>
///  Health Check Extensions
/// </summary>
public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddMemoryHealthCheck(
        this IHealthChecksBuilder builder,
        string name,
        HealthStatus? failureStatus = null,
        IEnumerable<string> tags = null)
    {
        return builder.AddCheck<MemoryHealthCheck>(name, failureStatus ?? HealthStatus.Degraded, tags);
    }
}

/// <summary>
///  Memory Health Check Implementation
/// </summary>
public class MemoryHealthCheck : IHealthCheck
{
    private readonly ILogger<MemoryHealthCheck> _logger;

    public MemoryHealthCheck(ILogger<MemoryHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var allocatedMemory = GC.GetTotalMemory(forceFullCollection: false);
        var memoryLimit = 1024L * 1024L * 1024L; // 1 GB

        var status = allocatedMemory < memoryLimit ? HealthStatus.Healthy : HealthStatus.Degraded;
        var description = $"目前記憶體使用量: {allocatedMemory / 1024 / 1024} MB";

        _logger.LogInformation(description);

        return Task.FromResult(new HealthCheckResult(
            status,
            description));
    }
}
