using BackendManagement.Domain.Common;

namespace BackendManagement.Domain.Entities;

/// <summary>
/// 使用者實體
/// </summary>
public class User : EntityBase
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密碼雜湊
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime LastLoginTime { get; set; }

    /// <summary>
    /// 刷新權杖
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 刷新權杖過期時間
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    /// <summary>
    /// 角色清單
    /// </summary>
    public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
} 