using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Application.Common.Models.Requests;
using BackendManagement.Application.Common.Models.Responses;

namespace BackendManagement.WebAPI.Controllers;

/// <summary>
/// 認證控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly ILogService _logService;

    public AuthController(
        IUserService userService,
        IJwtService jwtService,
        ILogService logService)
    {
        _userService = userService;
        _jwtService = jwtService;
        _logService = logService;
    }

    /// <summary>
    /// 登入
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                return Error("Invalid username or password");
            }

            var token = _jwtService.GenerateToken(user);
            return Success(new LoginResponse { Token = token });
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "Login failed");
            return Error("Login failed");
        }
    }

    /// <summary>
    /// 刷新權杖
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return Unauthorized(new ErrorResponse("無效的存取權杖"));
            }

            var userId = Guid.Parse(
                principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
            var user = await _userService.GetByIdAsync(userId);

            if (user == null ||
                user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new ErrorResponse("無效的刷新權杖"));
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userService.UpdateUserAsync(user);

            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "刷新權杖處理失敗");
            return BadRequest(new ErrorResponse("刷新權杖處理失敗"));
        }
    }
} 