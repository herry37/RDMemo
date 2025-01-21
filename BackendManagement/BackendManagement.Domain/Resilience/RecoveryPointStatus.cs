namespace BackendManagement.Domain.Resilience;

/// <summary>
/// 復原點狀態
/// </summary>
public enum RecoveryPointStatus
{
    /// <summary>
    /// 已建立
    /// </summary>
    Created,
    
    /// <summary>
    /// 可用
    /// </summary>
    Available,
    
    /// <summary>
    /// 失敗
    /// </summary>
    Failed,
    
    /// <summary>
    /// 無效
    /// </summary>
    Invalid,
    
    /// <summary>
    /// 已驗證
    /// </summary>
    Validated,
    
    /// <summary>
    /// 已復原
    /// </summary>
    Recovered
} 