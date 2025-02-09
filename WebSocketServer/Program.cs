using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using WebSocketServer.Services;

/// <summary>
///  Program 的主要進入點
/// </summary>
var builder = WebApplication.CreateBuilder(args);

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

/// <summary>
///  Configure Memory Cache
/// </summary>
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // 設定快取大小限制
    options.CompactionPercentage = 0.2; // 當達到限制時，移除 20% 的項目
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});

builder.Services.AddScoped<ITruckLocationService, TruckLocationService>();

/// <summary>
///  Configure HttpClient
/// </summary>
builder.Services.AddHttpClient("TruckLocation", client =>
{
    /// <summary>
    ///  設定 User-Agent
    /// </summary>
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
    /// <summary>
    ///  設定 Accept
    /// </summary>
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    /// <summary>
    ///  設定 Accept-Encoding
    /// </summary>
    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
    /// <summary>
    ///  設定 Timeout
    /// </summary>
    client.Timeout = TimeSpan.FromSeconds(60);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    /// <summary>
    ///  設定 ServerCertificateCustomValidationCallback
    /// </summary>
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    /// <summary>
    ///  設定 AutomaticDecompression
    /// </summary>
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
    /// <summary>
    ///  設定 UseProxy
    /// </summary>
    UseProxy = false,
    /// <summary>
    ///  設定 MaxConnectionsPerServer
    /// </summary>
    MaxConnectionsPerServer = 20
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5)); // 設定 Handler 生命週期

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
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
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

app.UseRouting();
app.UseCors();

// 在 app.UseStaticFiles() 之前加入
app.Use(async (context, next) =>
{
    var originalPath = context.Request.Path.Value ?? string.Empty;
    app.Logger.LogInformation("收到請求: {Path}", originalPath);

    // 處理 API 請求
    if (originalPath.StartsWith("/WebSocketServer/api/", StringComparison.OrdinalIgnoreCase))
    {
        var newPath = originalPath.Replace("/WebSocketServer/api/", "/api/", StringComparison.OrdinalIgnoreCase);
        app.Logger.LogInformation("API 路徑重寫: {OldPath} => {NewPath}", originalPath, newPath);
        context.Request.Path = newPath;
    }
    // 處理靜態檔案請求
    else if (originalPath.StartsWith("/WebSocketServer/", StringComparison.OrdinalIgnoreCase))
    {
        var newPath = originalPath.Replace("/WebSocketServer/", "/", StringComparison.OrdinalIgnoreCase);
        app.Logger.LogInformation("靜態檔案路徑重寫: {OldPath} => {NewPath}", originalPath, newPath);
        context.Request.Path = newPath;
    }

    await next();
});

// 配置靜態檔案
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 移除快取設定
        ctx.Context.Response.Headers.Remove("Cache-Control");
        ctx.Context.Response.Headers.Remove("Pragma");
        ctx.Context.Response.Headers.Remove("Expires");
        
        // 設定 CORS
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        
        // 設定內容安全性政策
        ctx.Context.Response.Headers["Content-Security-Policy"] = 
            "default-src 'self' 'unsafe-inline' 'unsafe-eval' https: data:;";
    }
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

/// <summary>
///  Configure Health Check Endpoint
/// </summary>
app.MapControllers();

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
