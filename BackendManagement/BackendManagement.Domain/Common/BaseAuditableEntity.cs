namespace BackendManagement.Domain.Common;

/// <summary>
/// 可稽核實體基礎類別
/// </summary>
public abstract class BaseAuditableEntity : EntityBase
{
    /// <summary>
    /// 建立時間
    /// </summary>
    public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 建立者
    /// </summary>
    public new string? CreatedBy { get; set; }

    /// <summary>
    /// 最後修改時間
    /// </summary>
    public new DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    public new string? LastModifiedBy { get; set; }
} 