namespace BackendManagement.Domain.MultiTenancy;

/// <summary>
/// 租戶資訊介面
/// </summary>
public interface ITenantInfo
{
    /// <summary>
    /// 租戶ID
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 租戶名稱
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 資料庫連線字串
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// 租戶屬性
    /// </summary>
    IDictionary<string, object> Properties { get; }
} 