using Prometheus;

namespace BackendManagement.Infrastructure.Monitoring.Metrics;

/// <summary>
/// 指標註冊器
/// </summary>
public static class MetricsRegistry
{
    private static readonly Counter _httpRequestsTotal = Prometheus.Metrics.CreateCounter(
        "http_requests_total",
        "Total number of HTTP requests",
        new[] { "method", "endpoint", "status" });

    private static readonly Histogram _httpRequestDuration = Prometheus.Metrics.CreateHistogram(
        "http_request_duration_seconds",
        "HTTP request duration in seconds",
        new[] { "method", "endpoint" });

    private static readonly Counter _databaseQueriesTotal = Prometheus.Metrics.CreateCounter(
        "database_queries_total",
        "Total number of database queries");

    private static readonly Counter _cacheHitsTotal = Prometheus.Metrics.CreateCounter(
        "cache_hits_total",
        "Total number of cache hits");

    private static readonly Gauge _activeConnections = Prometheus.Metrics.CreateGauge(
        "active_connections",
        "Number of active connections");

    public static void IncrementHttpRequests(string method, string endpoint, string status)
        => _httpRequestsTotal.WithLabels(method, endpoint, status).Inc();

    public static void ObserveHttpDuration(string method, string endpoint, double duration)
        => _httpRequestDuration.WithLabels(method, endpoint).Observe(duration);

    public static void IncrementDatabaseQueries() => _databaseQueriesTotal.Inc();

    public static void IncrementCacheHits() => _cacheHitsTotal.Inc();

    public static void SetActiveConnections(double count) => _activeConnections.Set(count);
} 