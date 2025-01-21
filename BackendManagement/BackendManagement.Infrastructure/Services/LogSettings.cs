namespace BackendManagement.Infrastructure.Services;

/// <summary>
/// 日誌設定
/// </summary>
public class LogSettings
{
    /// <summary>
    /// 日誌檔案根目錄
    /// </summary>
    public string LogDirectory { get; set; } = "Logs";

    /// <summary>
    /// 日誌保存天數
    /// </summary>
    public int RetentionDays { get; set; } = 30;
} 