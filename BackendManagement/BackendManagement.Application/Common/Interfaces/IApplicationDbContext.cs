namespace BackendManagement.Application.Common.Interfaces;

/// <summary>
/// 應用程式資料庫上下文介面
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// 儲存異動
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 