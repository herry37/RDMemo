using BackendManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BackendManagement.Infrastructure.Services;

/// <summary>
/// 日誌服務實作
/// </summary>
public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;

    public LogService(ILogger<LogService> logger)
    {
        _logger = logger;
    }

    public void Information(string message)
    {
        _logger.LogInformation(message);
    }

    public void Warning(string message)
    {
        _logger.LogWarning(message);
    }

    public void Error(Exception exception, string message)
    {
        _logger.LogError(exception, message);
    }
} 