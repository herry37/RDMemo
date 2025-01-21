namespace BackendManagement.Application.Common.AsyncOperation;

/// <summary>
/// 操作狀態列舉
/// </summary>
public enum OperationStatus
{
    /// <summary>
    /// 未找到
    /// </summary>
    NotFound,

    /// <summary>
    /// 等待中
    /// </summary>
    Pending,

    /// <summary>
    /// 執行中
    /// </summary>
    Running,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed,

    /// <summary>
    /// 已失敗
    /// </summary>
    Failed,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
} 