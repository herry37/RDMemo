namespace BackendManagement.Domain.Common;

/// <summary>
/// 可稽核實體介面
/// </summary>
public interface IAuditableEntity : IEntity
{
    /// <summary>
    /// 建立時間
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// 最後修改時間
    /// </summary>
    DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    string? LastModifiedBy { get; set; }
} 