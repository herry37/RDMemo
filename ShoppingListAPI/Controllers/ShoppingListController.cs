using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services.FileDb;

namespace ShoppingListAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]  // 使用固定窗口限制器
public class ShoppingListController : ControllerBase
{
    private readonly IFileDbService _fileDbService;
    private readonly ILogger<ShoppingListController> _logger;

    public ShoppingListController(
        IFileDbService fileDbService,
        ILogger<ShoppingListController> logger)
    {
        _fileDbService = fileDbService;
        _logger = logger;
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
            if (page < 1) return BadRequest("頁碼必須大於0");
            if (pageSize < 1 || pageSize > 100) return BadRequest("每頁筆數必須在1到100之間");

            var lists = await _fileDbService.GetAllShoppingListsAsync(page, pageSize);
            return Ok(lists);
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> AddItem(string id, [FromBody] ShoppingItem item)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("清單ID不能為空");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedList = await _fileDbService.AddItemToListAsync(id, item);
            return Ok(updatedList);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增購物項目時發生錯誤");
            return StatusCode(500, "新增購物項目時發生錯誤");
        }
    }

    /// <summary>
    /// 切換項目完成狀態
    /// </summary>
    [HttpPut("{listId}/items/{itemId}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> ToggleItem(string listId, string itemId)
    {
        try
        {
            if (string.IsNullOrEmpty(listId))
            {
                return BadRequest("清單ID不能為空");
            }

            if (string.IsNullOrEmpty(itemId))
            {
                return BadRequest("項目ID不能為空");
            }

            var updatedList = await _fileDbService.ToggleItemAsync(listId, itemId);
            return Ok(updatedList);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切換項目狀態時發生錯誤");
            return StatusCode(500, "切換項目狀態時發生錯誤");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            await _fileDbService.DeleteShoppingListAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除購物清單時發生錯誤");
            return StatusCode(500, "刪除購物清單時發生錯誤");
        }
    }

    public class BulkDeleteRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    /// <summary>
    /// 批量刪除購物清單
    /// </summary>
    [HttpDelete("bulk-delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
    {
        try
        {
            if (!DateTime.TryParse(request.StartDate, out var startDate) || 
                !DateTime.TryParse(request.EndDate, out var endDate))
            {
                return BadRequest("日期格式無效");
            }

            if (startDate > endDate)
            {
                return BadRequest("開始日期不能晚於結束日期");
            }

            // 取得所有清單
            var lists = await _fileDbService.GetAllAsync();
            
            // 過濾出要刪除的清單
            var listsToDelete = lists.Where(list => 
                DateTime.TryParse(list.BuyData, out var buyDate) && 
                buyDate >= startDate && 
                buyDate <= endDate
            ).ToList();

            // 批量刪除
            foreach (var list in listsToDelete)
            {
                await _fileDbService.DeleteAsync(list.Id);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量刪除購物清單時發生錯誤");
            return StatusCode(500, "批量刪除購物清單時發生錯誤");
        }
    }
}