using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services.FileDb;
using ShoppingListAPI.Services.WebSocket;
using System.Text.Json;

namespace ShoppingListAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]  // 使用固定窗口限制器
public class ShoppingListController : ControllerBase
{
    private readonly IFileDbService _fileDbService;
    private readonly ILogger<ShoppingListController> _logger;
    private readonly WebSocketHandler _webSocketHandler;

    public ShoppingListController(
        IFileDbService fileDbService,
        ILogger<ShoppingListController> logger,
        WebSocketHandler webSocketHandler)
    {
        _fileDbService = fileDbService;
        _logger = logger;
        _webSocketHandler = webSocketHandler;
    }

    /// <summary>
    /// 取得所有購物清單
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ShoppingList>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation($"開始取得購物清單，頁碼：{page}，每頁筆數：{pageSize}");
            
            if (page < 1) 
            {
                _logger.LogWarning($"無效的頁碼：{page}");
                return BadRequest("頁碼必須大於0");
            }
            
            if (pageSize < 1 || pageSize > 100) 
            {
                _logger.LogWarning($"無效的每頁筆數：{pageSize}");
                return BadRequest("每頁筆數必須在1到100之間");
            }

            var lists = await _fileDbService.GetAllAsync();
            _logger.LogInformation($"成功取得 {lists.Count} 個購物清單");

            // 分頁
            var pagedLists = lists
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(pagedLists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得購物清單時發生錯誤");
            return StatusCode(500, "取得購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 取得指定的購物清單
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> Get(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID不能為空");
            }

            var list = await _fileDbService.GetShoppingListAsync(id);
            return Ok(list);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得購物清單時發生錯誤");
            return StatusCode(500, "取得購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 建立新的購物清單
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> Create([FromBody] ShoppingList list)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdList = await _fileDbService.CreateShoppingListAsync(list);
            await _webSocketHandler.BroadcastShoppingListUpdate(createdList);
            return CreatedAtAction(nameof(Get), new { id = createdList.Id }, createdList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立購物清單時發生錯誤");
            return StatusCode(500, "建立購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 新增項目到購物清單
    /// </summary>
    [HttpPost("{id}/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> AddItem(string id, [FromBody] ShoppingItem item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedList = await _fileDbService.AddItemToListAsync(id, item);
            await _webSocketHandler.BroadcastShoppingListUpdate(updatedList);
            return Ok(updatedList);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增項目時發生錯誤");
            return StatusCode(500, "新增項目時發生錯誤");
        }
    }

    /// <summary>
    /// 切換購物項目的完成狀態
    /// </summary>
    [HttpPost("{listId}/items/{itemId}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> ToggleItem(string listId, string itemId)
    {
        try
        {
            _logger.LogInformation($"切換購物項目狀態：清單ID={listId}，項目ID={itemId}");
            
            if (string.IsNullOrEmpty(listId) || string.IsNullOrEmpty(itemId))
            {
                return BadRequest("清單ID和項目ID不能為空");
            }

            var list = await _fileDbService.ToggleItemAsync(listId, itemId);
            
            // 通知所有連接的客戶端
            var message = new
            {
                Type = "ItemToggled",
                Data = new { ListId = listId, ItemId = itemId, List = list }
            };
            await _webSocketHandler.BroadcastAsync(JsonSerializer.Serialize(message));

            return Ok(list);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切換購物項目狀態時發生錯誤");
            return StatusCode(500, "切換購物項目狀態時發生錯誤");
        }
    }

    /// <summary>
    /// 刪除購物清單
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _fileDbService.DeleteShoppingListAsync(id);
            await _webSocketHandler.BroadcastMessage(JsonSerializer.Serialize(new { type = "shoppinglist_delete", data = id }));
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除購物清單時發生錯誤");
            return StatusCode(500, "刪除購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 批量刪除購物清單
    /// </summary>
    [HttpDelete("bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkDelete([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                return BadRequest("開始日期不能大於結束日期");
            }

            // 取得所有清單
            var lists = await _fileDbService.GetAllAsync();
            
            // 過濾出要刪除的清單
            var listsToDelete = lists.Where(list => 
                list.BuyDate.HasValue && 
                list.BuyDate.Value.Date >= startDate.Date && 
                list.BuyDate.Value.Date <= endDate.Date
            ).ToList();

            // 批量刪除
            foreach (var list in listsToDelete)
            {
                await _fileDbService.DeleteAsync(list.Id);
                await _webSocketHandler.BroadcastMessage(JsonSerializer.Serialize(new { type = "shoppinglist_delete", data = list.Id }));
            }

            return Ok(new { message = $"成功刪除 {listsToDelete.Count} 個購物清單" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量刪除購物清單時發生錯誤");
            return StatusCode(500, "批量刪除購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 批量刪除指定年月的購物清單
    /// </summary>
    [HttpDelete("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BatchDeleteResult>> BatchDelete([FromBody] BatchDeleteModel model)
    {
        try
        {
            _logger.LogInformation($"開始批量刪除 {model.Year}年{model.Month}月 的購物清單");
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var count = await _fileDbService.BatchDeleteAsync(model.Year, model.Month);
            
            // 通知所有連接的客戶端
            var message = new
            {
                Type = "BatchDelete",
                Data = new { Year = model.Year, Month = model.Month, Count = count }
            };
            await _webSocketHandler.BroadcastAsync(JsonSerializer.Serialize(message));

            return Ok(new BatchDeleteResult 
            { 
                DeletedCount = count,
                Message = $"成功刪除 {count} 筆購物清單"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量刪除購物清單時發生錯誤");
            return StatusCode(500, "批量刪除購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 搜尋購物清單
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ShoppingList>>> Search(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? title)
    {
        try
        {
            _logger.LogInformation($"開始搜尋購物清單，起始日期：{startDate}，結束日期：{endDate}，標題：{title}");
            
            var lists = await _fileDbService.GetAllAsync();

            // 根據條件篩選
            var query = lists.AsQueryable();

            if (startDate.HasValue)
            {
                var startDateValue = startDate.Value.Date;
                query = query.Where(x => x.BuyDate.HasValue && x.BuyDate.Value.Date >= startDateValue);
                _logger.LogInformation($"套用起始日期過濾：{startDateValue:yyyy-MM-dd}");
            }

            if (endDate.HasValue)
            {
                var endDateValue = endDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.BuyDate.HasValue && x.BuyDate.Value.Date <= endDateValue);
                _logger.LogInformation($"套用結束日期過濾：{endDateValue:yyyy-MM-dd}");
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
                _logger.LogInformation($"套用標題過濾：{title}");
            }

            // 依購買日期排序
            var result = query
                .OrderByDescending(x => x.BuyDate)
                .ToList();

            _logger.LogInformation($"搜尋完成，找到 {result.Count} 筆符合條件的清單");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋購物清單時發生錯誤");
            return StatusCode(500, "搜尋購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 更新購物清單
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(string id, [FromBody] ShoppingListUpdateRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID不能為空");
            }

            var list = await _fileDbService.GetShoppingListAsync(id);
            list.Items = request.Items;

            await _fileDbService.UpdateShoppingListAsync(list);

            // 通知其他客戶端
            await _webSocketHandler.BroadcastShoppingListUpdate(list);

            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新購物清單時發生錯誤");
            return StatusCode(500, "更新購物清單時發生錯誤");
        }
    }
}