using BackendManagement.Domain.Entities;

namespace BackendManagement.Application.Common.Interfaces;

/// <summary>
/// 使用者服務介面
/// </summary>
public interface IUserService : IApplicationService
{
    /// <summary>
    /// 建立使用者
    /// </summary>
    Task<User> CreateUserAsync(User user, string password);

    /// <summary>
    /// 驗證使用者
    /// </summary>
    Task<bool> ValidateUserAsync(string username, string password);

    /// <summary>
    /// 更新使用者資料
    /// </summary>
    Task UpdateAsync(User user);

    /// <summary>
    /// 取得使用者資料
    /// </summary>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// 根據 ID 取得使用者
    /// </summary>
    Task<User?> GetByIdAsync(Guid id);
} 