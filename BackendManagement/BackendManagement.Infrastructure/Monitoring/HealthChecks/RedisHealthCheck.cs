namespace BackendManagement.Infrastructure.Monitoring.HealthChecks;

/// <summary>
/// Redis健康檢查
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisHealthCheck> _logger;

    public RedisHealthCheck(
        IConnectionMultiplexer redis,
        ILogger<RedisHealthCheck> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var latency = await db.PingAsync();

            var data = new Dictionary<string, object>
            {
                { "Latency", latency.TotalMilliseconds }
            };

            return latency.TotalMilliseconds < 100
                ? HealthCheckResult.Healthy("Redis運作正常", data)
                : HealthCheckResult.Degraded("Redis延遲過高", data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis健康檢查失敗");
            return HealthCheckResult.Unhealthy("Redis健康檢查失敗", ex);
        }
    }
} 