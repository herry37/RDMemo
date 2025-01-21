namespace BackendManagement.Infrastructure.Authentication;

/// <summary>
/// JWT設定
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 密鑰
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 發行者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 接收者
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 存取權杖有效期限（分鐘）
    /// </summary>
    public int AccessTokenExpiryMinutes { get; set; } = 30;

    /// <summary>
    /// 刷新權杖有效期限（天）
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
} 