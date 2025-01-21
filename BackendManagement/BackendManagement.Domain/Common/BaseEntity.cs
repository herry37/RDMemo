namespace BackendManagement.Domain.Common;

/// <summary>
/// 實體基礎類別
/// </summary>
public abstract class BaseEntity : IEntity
{
    /// <summary>
    /// 實體識別碼
    /// </summary>
    public Guid Id { get; set; }
} 