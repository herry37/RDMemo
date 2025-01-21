using Microsoft.AspNetCore.Mvc;

namespace BackendManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Success<T>(T data) => Ok(data);
    protected IActionResult Error(string message) => BadRequest(new { error = message });
    protected IActionResult NotFound(string message) => NotFound(new { error = message });
} 