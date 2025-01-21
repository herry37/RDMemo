using BackendManagement.Domain.Common;

namespace BackendManagement.Domain.Resilience;

/// <summary>
/// 復原點
/// </summary>
public class RecoveryPoint : EntityBase
{
    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 備份路徑
    /// </summary>
    public string BackupPath { get; set; } = string.Empty;

    /// <summary>
    /// 檢查碼
    /// </summary>
    public string? Checksum { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public RecoveryPointStatus Status { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 驗證時間
    /// </summary>
    public DateTime? ValidatedAt { get; set; }

    /// <summary>
    /// 復原時間
    /// </summary>
    public DateTime? RecoveredAt { get; set; }
} 