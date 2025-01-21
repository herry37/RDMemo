var builder = WebApplication.CreateBuilder(args);

// 加入基礎服務
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 設定資料庫
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

// 移除重複的 DbContext 註冊，使用 Infrastructure 層的註冊
builder.Services.AddInfrastructureServices(builder.Configuration);

// 設定 Serilog
builder.Host.UseSerilog((context, configuration) => 
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["Elasticsearch:Url"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"backend-management-{DateTime.UtcNow:yyyy-MM}"
        });
});

// 加入安全性設定
builder.Services.Configure<SecurityOptions>(
    builder.Configuration.GetSection("Security"));

// 加入安全性中介軟體
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddCors(options =>
{
    var corsPolicy = builder.Configuration
        .GetSection("Security:CorsPolicy")
        .Get<CorsPolicy>();

    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(corsPolicy!.AllowedOrigins)
               .WithMethods(corsPolicy.AllowedMethods)
               .WithHeaders(corsPolicy.AllowedHeaders)
               .WithExposedHeaders("X-Pagination");
    });
});

// 加入效能優化設定
builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromMinutes(10)));
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// 加入背景服務
builder.Services.AddHostedService<BackupHostedService>();

// 加入多租戶支援
builder.Services.AddScoped<ITenantService, TenantService>();

// 在其他服務註冊之後，加入核心服務
builder.Services.AddScoped<IDateTime, DateTimeService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<IDomainEventService, DomainEventService>();
builder.Services.AddScoped<IDisasterRecoveryService, DisasterRecoveryService>();
builder.Services.AddScoped<IDataSynchronizer, DataSynchronizer>();

var app = builder.Build();

// 資料庫初始化
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("資料庫遷移完成");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "資料庫遷移失敗");
        throw;
    }
}

// 註冊中介軟體（按照正確順序）
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts();
app.UseCors();
app.UseRateLimiter();

// 身分驗證相關
app.UseAuthentication();
app.UseAuthorization();

// 效能優化相關
app.UseResponseCaching();
app.UseOutputCache();
app.UseResponseCompression();

app.MapControllers();

// 加入健康檢查端點
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var result = new
        {
            Status = report.Status.ToString(),
            Duration = report.TotalDuration,
            Info = report.Entries.Select(e => new
            {
                Key = e.Key,
                Status = e.Value.Status.ToString(),
                Duration = e.Value.Duration,
                Description = e.Value.Description
            })
        };
        
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(result);
    }
});

// 加入額外的安全性標頭
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Permissions-Policy", 
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self'; frame-ancestors 'none';");
    
    await next();
});

app.Run(); 