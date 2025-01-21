using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BackendManagement.Infrastructure.Persistence;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseDatabase(
        this DbContextOptionsBuilder builder,
        string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        // 設定資料庫
        builder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                mysqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
                mysqlOptions.EnableIndexOptimizedBooleanColumns();
            }
        );

        // 加入查詢追蹤選項
        builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        return builder;
    }
} 