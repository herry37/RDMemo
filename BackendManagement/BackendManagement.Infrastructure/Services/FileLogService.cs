namespace BackendManagement.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BackendManagement.Application.Common.Interfaces;

/// <summary>
/// 檔案日誌服務
/// </summary>
public class FileLogService : ILogService
{
    private readonly string _logPath;
    private readonly ILogger<FileLogService> _logger;

    public FileLogService(IConfiguration configuration, ILogger<FileLogService> logger)
    {
        _logPath = configuration["Logging:FilePath"] ?? "logs";
        _logger = logger;
        
        if (!Directory.Exists(_logPath))
        {
            Directory.CreateDirectory(_logPath);
        }
    }

    public void Information(string message) => _logger.LogInformation(message);
    public void Warning(string message) => _logger.LogWarning(message);
    public void Error(Exception ex, string message) => _logger.LogError(ex, message);

    public void Information(string message, params object[] args)
    {
        WriteLog("INFO", string.Format(message, args));
        _logger.LogInformation(message, args);
    }

    public void Warning(string message, params object[] args)
    {
        WriteLog("WARN", string.Format(message, args));
        _logger.LogWarning(message, args);
    }

    public void Error(string message, params object[] args)
    {
        WriteLog("ERROR", string.Format(message, args));
        _logger.LogError(message, args);
    }

    public void Error(Exception exception, string message, params object[] args)
    {
        var formattedMessage = $"{string.Format(message, args)}\n{exception}";
        WriteLog("ERROR", formattedMessage);
        _logger.LogError(exception, message, args);
    }

    private void WriteLog(string level, string message)
    {
        var logFile = Path.Combine(_logPath, $"{DateTime.UtcNow:yyyy-MM-dd}.log");
        var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";
        
        try
        {
            File.AppendAllLines(logFile, new[] { logEntry });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to log file");
        }
    }
} 