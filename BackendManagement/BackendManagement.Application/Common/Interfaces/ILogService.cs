namespace BackendManagement.Application.Common.Interfaces;

/// <summary>
/// 日誌服務介面
/// </summary>
public interface ILogService
{
    /// <summary>
    /// 記錄資訊
    /// </summary>
    void Information(string message);
    
    /// <summary>
    /// 記錄警告
    /// </summary>
    void Warning(string message);
    
    /// <summary>
    /// 記錄錯誤
    /// </summary>
    void Error(Exception ex, string message);
} 