namespace BackendManagement.WebAPI.Controllers;

/// <summary>
/// 災難復原控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class DisasterRecoveryController : ControllerBase
{
    private readonly IDisasterRecoveryService _recoveryService;
    private readonly ILogger<DisasterRecoveryController> _logger;

    public DisasterRecoveryController(
        IDisasterRecoveryService recoveryService,
        ILogger<DisasterRecoveryController> logger)
    {
        _recoveryService = recoveryService;
        _logger = logger;
    }

    /// <summary>
    /// 建立復原點
    /// </summary>
    [HttpPost("recovery-points")]
    public async Task<IActionResult> CreateRecoveryPoint(
        [FromBody] CreateRecoveryPointRequest request)
    {
        try
        {
            var recoveryPoint = await _recoveryService.CreateRecoveryPointAsync(
                request.Description);

            return Ok(recoveryPoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立復原點失敗");
            return StatusCode(500, "建立復原點失敗");
        }
    }

    /// <summary>
    /// 從復原點還原
    /// </summary>
    [HttpPost("recovery-points/{id}/recover")]
    public async Task<IActionResult> RecoverFromPoint(Guid id)
    {
        try
        {
            var recoveryPoint = await _recoveryService.GetRecoveryPointAsync(id);
            if (recoveryPoint == null)
            {
                return NotFound();
            }

            await _recoveryService.RecoverFromPointAsync(recoveryPoint);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "從復原點還原失敗");
            return StatusCode(500, "從復原點還原失敗");
        }
    }

    /// <summary>
    /// 驗證復原點
    /// </summary>
    [HttpPost("recovery-points/{id}/validate")]
    public async Task<IActionResult> ValidateRecoveryPoint(Guid id)
    {
        try
        {
            var recoveryPoint = await _recoveryService.GetRecoveryPointAsync(id);
            if (recoveryPoint == null)
            {
                return NotFound();
            }

            await _recoveryService.ValidateRecoveryPointAsync(recoveryPoint);
            return Ok(recoveryPoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "驗證復原點失敗");
            return StatusCode(500, "驗證復原點失敗");
        }
    }
} 