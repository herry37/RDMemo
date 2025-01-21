namespace BackendManagement.Application.Common.Interfaces;

public interface IAsyncOperation<T>
{
    /// <summary>
    /// 操作識別碼
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// 操作狀態
    /// </summary>
    OperationStatus Status { get; }

    /// <summary>
    /// 執行操作
    /// </summary>
    Task<T> ExecuteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消操作
    /// </summary>
    Task<bool> CancelAsync();

    /// <summary>
    /// 取得結果
    /// </summary>
    Task<T> GetResultAsync();
}

/// <summary>
/// 操作狀態
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
    /// 已取消
    /// </summary>
    Cancelled,

    /// <summary>
    /// 失敗
    /// </summary>
    Failed
} 