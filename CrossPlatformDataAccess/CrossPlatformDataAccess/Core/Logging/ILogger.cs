using System;

namespace CrossPlatformDataAccess.Core.Logging
{
    /// <summary>
    /// 日誌記錄介面
    /// </summary>
    public interface ILogger
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, Exception exception = null, params object[] args);
        void LogDebug(string message, params object[] args);
    }
}
