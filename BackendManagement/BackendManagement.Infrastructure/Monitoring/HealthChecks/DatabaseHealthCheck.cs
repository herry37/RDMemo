namespace BackendManagement.Infrastructure.Monitoring.HealthChecks;

/// <summary>
/// 資料庫健康檢查
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(
        ApplicationDbContext context,
        ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 檢查資料庫連線
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("無法連接到資料庫");
            }

            // 檢查資料庫遷移狀態
            var pendingMigrations = await _context.Database
                .GetPendingMigrationsAsync(cancellationToken);
            if (pendingMigrations.Any())
            {
                return HealthCheckResult.Degraded(
                    "有待執行的資料庫遷移",
                    data: new Dictionary<string, object>
                    {
                        { "PendingMigrations", pendingMigrations }
                    });
            }

            return HealthCheckResult.Healthy("資料庫運作正常");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "資料庫健康檢查失敗");
            return HealthCheckResult.Unhealthy("資料庫健康檢查失敗", ex);
        }
    }
} 