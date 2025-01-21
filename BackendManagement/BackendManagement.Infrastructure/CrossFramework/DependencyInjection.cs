using BackendManagement.Infrastructure.Persistence;
using BackendManagement.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendManagement.Infrastructure.CrossFramework;

/// <summary>
/// 相依注入擴充方法
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 加入後台管理系統服務
    /// </summary>
    public static IServiceCollection AddBackendManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 註冊資料庫
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string is not configured.");
            
            // 直接配置資料庫
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mysqlOptions =>
                {
                    mysqlOptions.EnableRetryOnFailure();
                    mysqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
                    mysqlOptions.EnableIndexOptimizedBooleanColumns();
                }
            );

            // 加入攔截器
            var currentUserService = serviceProvider.GetRequiredService<ICurrentUserService>();
            var dateTime = serviceProvider.GetRequiredService<Application.Common.Interfaces.IDateTime>();
            options.AddInterceptors(new AuditableEntitySaveChangesInterceptor(currentUserService, dateTime));
        });

        // 註冊 Redis
        services.AddStackExchangeRedisCache(options =>
        {
            var redisConnection = configuration.GetConnectionString("Redis") 
                ?? throw new InvalidOperationException("Redis connection string is not configured.");
            options.Configuration = redisConnection;
            options.InstanceName = "BackendManagement:";
        });

        // 註冊效能優化相關服務
        services.AddMemoryCache();
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        // 註冊服務
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<ILogService, FileLogService>();
        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
        services.AddScoped<Application.Common.Interfaces.IDateTime, DateTimeService>();  // 明確指定介面

        // 使用快取裝飾器包裝Repository
        services.Decorate(typeof(IRepository<>), typeof(CachedRepository<>));

        // 註冊 MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
} 