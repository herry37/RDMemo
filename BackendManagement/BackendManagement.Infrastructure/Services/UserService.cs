using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DomainInterfaces = BackendManagement.Domain.Interfaces;

namespace BackendManagement.Infrastructure.Services;

/// <summary>
/// 使用者服務實作
/// </summary>
public class UserService : IUserService
{
    private readonly DomainInterfaces.IRepository<User> _userRepository;
    private readonly ILogService _logService;

    public UserService(
        DomainInterfaces.IRepository<User> userRepository,
        ILogService logService)
    {
        _userRepository = userRepository;
        _logService = logService;
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        try
        {
            user.PasswordHash = HashPassword(password);
            var result = await _userRepository.AddAsync(user);
            _logService.Information($"已建立使用者 {user.Username}");
            return result;
        }
        catch (Exception ex)
        {
            _logService.Error(ex, $"建立使用者 {user.Username} 失敗");
            throw;
        }
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var user = await GetByUsernameAsync(username);
        if (user == null)
        {
            return false;
        }

        var isValid = VerifyPassword(password, user.PasswordHash);
        if (isValid)
        {
            user.LastLoginTime = DateTime.UtcNow;
            await UpdateAsync(user);
            _logService.Information($"使用者 {username} 登入成功");
        }
        else
        {
            _logService.Warning($"使用者 {username} 登入失敗");
        }

        return isValid;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            return users.FirstOrDefault(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logService.Error(ex, $"取得使用者 {username} 資料失敗");
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _userRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logService.Error(ex, $"取得使用者 ID:{id} 資料失敗");
            throw;
        }
    }

    public async Task UpdateAsync(User user)
    {
        try
        {
            await _userRepository.UpdateAsync(user);
            _logService.Information($"已更新使用者 {user.Username} 的資料");
        }
        catch (Exception ex)
        {
            _logService.Error(ex, $"更新使用者 {user.Username} 資料失敗");
            throw;
        }
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
} 