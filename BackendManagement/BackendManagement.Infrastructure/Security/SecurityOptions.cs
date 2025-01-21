namespace BackendManagement.Infrastructure.Security;

/// <summary>
/// 安全性設定
/// </summary>
public class SecurityOptions
{
    /// <summary>
    /// 密碼政策
    /// </summary>
    public PasswordPolicy PasswordPolicy { get; set; } = new();

    /// <summary>
    /// 登入失敗鎖定政策
    /// </summary>
    public LockoutPolicy LockoutPolicy { get; set; } = new();

    /// <summary>
    /// CORS政策
    /// </summary>
    public CorsPolicy CorsPolicy { get; set; } = new();
}

public class PasswordPolicy
{
    public int MinimumLength { get; set; } = 8;
    public bool RequireDigit { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireNonAlphanumeric { get; set; } = true;
}

public class LockoutPolicy
{
    public int MaxFailedAttempts { get; set; } = 5;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
}

public class CorsPolicy
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();
    public string[] AllowedHeaders { get; set; } = Array.Empty<string>();
} 