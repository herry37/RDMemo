using BackendManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BackendManagement.Infrastructure.Logging;

/// <summary>
/// Elasticsearch日誌服務
/// </summary>
public class ElasticLogService : ILogService
{
    private readonly ILogger<ElasticLogService> _logger;

    public ElasticLogService(ILogger<ElasticLogService> logger)
    {
        _logger = logger;
    }

    public void Information(string message)
    {
        _logger.LogInformation(message);
        // TODO: 實作 Elastic 日誌記錄
    }

    public void Warning(string message)
    {
        _logger.LogWarning(message);
        // TODO: 實作 Elastic 日誌記錄
    }

    public void Error(Exception exception, string message)
    {
        _logger.LogError(exception, message);
        // TODO: 實作 Elastic 日誌記錄
    }
} 