namespace BackendManagement.Domain.Common;

/// <summary>
/// 定義實體的基本介面
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 實體識別碼
    /// </summary>
    Guid Id { get; set; }
} 