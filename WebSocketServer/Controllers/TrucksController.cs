using Microsoft.AspNetCore.Mvc;
using WebSocketServer.Services;

namespace WebSocketServer.Controllers;

/// <summary>
/// 取得垃圾車位置控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TrucksController : ControllerBase
{
    private readonly ITruckLocationService _truckLocationService;
    private readonly ILogger<TrucksController> _logger;

    /// <summary>
    /// 建構函數
    /// </summary>
    /// <param name="truckLocationService">垃圾車位置服務</param>
    /// <param name="logger">日誌記錄器</param>
    public TrucksController(ITruckLocationService truckLocationService, ILogger<TrucksController> logger)
    {
        _truckLocationService = truckLocationService;
        _logger = logger;
    }

    /// <summary>
    /// 取得垃圾車位置
    /// </summary>
    /// <param name="cancellationToken">取消代碼</param>
    /// <returns>垃圾車位置</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("收到垃圾車位置請求");
            var trucks = await _truckLocationService.GetTruckLocationsAsync(cancellationToken);

            if (trucks == null || !trucks.Any())
            {
                _logger.LogWarning("沒有垃圾車資料");
                return Ok(new { success = true, message = "目前沒有垃圾車資料", data = new List<object>() });
            }

            // 確保回傳的是 JSON 物件
            var result = new { success = true, data = trucks };
            _logger.LogInformation("成功獲取 {Count} 筆垃圾車位置資料", trucks.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取垃圾車位置時發生錯誤");
            return StatusCode(500, new { success = false, message = "內部伺服器錯誤" });
        }
    }
}
