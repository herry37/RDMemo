using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Application.Common.Models.Requests;
using BackendManagement.Application.Common.Models.Responses;

namespace BackendManagement.WebAPI.Controllers;

public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;

    public UsersController(
        IUserService userService,
        ILogService logService)
    {
        _userService = userService;
        _logService = logService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var user = await _userService.RegisterUserAsync(request);
            return Success(user);
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "Registration failed");
            return Error("Registration failed");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                return Error("Invalid username or password");
            }

            return Success(user);
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "Login failed");
            return Error("Login failed");
        }
    }
} 