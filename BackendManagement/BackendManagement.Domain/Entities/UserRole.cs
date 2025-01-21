using BackendManagement.Domain.Common;

namespace BackendManagement.Domain.Entities;

/// <summary>
/// 使用者角色實體
/// </summary>
public class UserRole : EntityBase
{
    /// <summary>
    /// 角色名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 權限清單
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// 使用者清單
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
} 