namespace BackendManagement.WebAPI.Controllers;

/// <summary>
/// 儀表板控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IElasticClient _elasticClient;
    private readonly IMetricsRoot _metrics;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IElasticClient elasticClient,
        IMetricsRoot metrics,
        ILogger<DashboardController> logger)
    {
        _elasticClient = elasticClient;
        _metrics = metrics;
        _logger = logger;
    }

    /// <summary>
    /// 取得系統概況
    /// </summary>
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        try
        {
            var now = DateTime.UtcNow;
            var start = now.AddHours(-24);

            // 取得系統指標
            var metrics = new
            {
                ActiveConnections = MetricsRegistry.ActiveConnections.Value,
                DatabaseOperations = MetricsRegistry.DatabaseOperations.Value,
                CacheHits = MetricsRegistry.CacheHits.Value,
                CacheMisses = MetricsRegistry.CacheMisses.Value
            };

            // 取得錯誤日誌
            var errorLogs = await _elasticClient.SearchAsync<LogEntry>(s => s
                .Index($"{_indexPrefix}-*")
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Term(t => t.Level, "Error"),
                            m => m.DateRange(r => r
                                .Field(f => f.Timestamp)
                                .GreaterThanOrEquals(start)
                                .LessThanOrEquals(now)
                            )
                        )
                    )
                )
                .Size(10)
                .Sort(sort => sort.Descending(f => f.Timestamp))
            );

            // 取得效能統計
            var performanceStats = await _elasticClient.SearchAsync<LogEntry>(s => s
                .Index($"{_indexPrefix}-*")
                .Aggregations(a => a
                    .DateHistogram("requests_over_time", h => h
                        .Field(f => f.Timestamp)
                        .FixedInterval(new Time("1h"))
                        .MinimumDocumentCount(0)
                    )
                    .Average("avg_response_time", avg => avg
                        .Field("AdditionalData.ResponseTime")
                    )
                )
            );

            return Ok(new
            {
                Metrics = metrics,
                ErrorLogs = errorLogs.Documents,
                PerformanceStats = new
                {
                    RequestsOverTime = performanceStats.Aggregations
                        .DateHistogram("requests_over_time"),
                    AverageResponseTime = performanceStats.Aggregations
                        .Average("avg_response_time").Value
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得系統概況失敗");
            return StatusCode(500, "取得系統概況失敗");
        }
    }

    /// <summary>
    /// 取得資源使用狀況
    /// </summary>
    [HttpGet("resources")]
    public IActionResult GetResources()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var resources = new
            {
                CPU = new
                {
                    ProcessorCount = Environment.ProcessorCount,
                    ProcessorTime = process.TotalProcessorTime,
                    UserProcessorTime = process.UserProcessorTime
                },
                Memory = new
                {
                    TotalMemory = GC.GetTotalMemory(false),
                    WorkingSet = process.WorkingSet64,
                    PrivateMemory = process.PrivateMemorySize64,
                    Gen0Collections = GC.CollectionCount(0),
                    Gen1Collections = GC.CollectionCount(1),
                    Gen2Collections = GC.CollectionCount(2)
                },
                Thread = new
                {
                    ThreadCount = process.Threads.Count,
                    HandleCount = process.HandleCount
                }
            };

            return Ok(resources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得資源使用狀況失敗");
            return StatusCode(500, "取得資源使用狀況失敗");
        }
    }

    /// <summary>
    /// 取得租戶統計
    /// </summary>
    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenantStats()
    {
        try
        {
            var now = DateTime.UtcNow;
            var start = now.AddDays(-30);

            var stats = await _elasticClient.SearchAsync<LogEntry>(s => s
                .Index($"{_indexPrefix}-*")
                .Aggregations(a => a
                    .Terms("tenants", t => t
                        .Field("AdditionalData.TenantId")
                        .Size(100)
                        .Aggregations(aa => aa
                            .DateHistogram("requests_over_time", h => h
                                .Field(f => f.Timestamp)
                                .FixedInterval(new Time("1d"))
                                .MinimumDocumentCount(0)
                            )
                            .Average("avg_response_time", avg => avg
                                .Field("AdditionalData.ResponseTime")
                            )
                        )
                    )
                )
            );

            return Ok(stats.Aggregations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得租戶統計失敗");
            return StatusCode(500, "取得租戶統計失敗");
        }
    }
} 