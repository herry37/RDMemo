using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services.FileDb;
using System.Text.Json;

namespace ShoppingListAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<List<ShoppingList>>> GetAllLists()
    {
        try
        {
            var lists = await _fileDbService.GetAllShoppingListsAsync();
            return Ok(lists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得購物清單失敗");
            return StatusCode(500, "取得購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 取得指定的購物清單
    /// </summary>
    [HttpGet("{listId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> GetList(string listId)
    {
        try
        {
            var list = await _fileDbService.GetShoppingListById(listId);
            if (list == null)
            {
                return NotFound($"找不到ID為 {listId} 的購物清單");
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得購物清單 {ListId} 失敗", listId);
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
    public async Task<ActionResult<ShoppingList>> CreateList([FromBody] ShoppingList list)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(list.Title))
            {
                return BadRequest("清單標題不能為空");
            }

            list.Id = Guid.NewGuid().ToString();
            list.CreatedAt = DateTime.UtcNow;
            list.Items ??= new List<ShoppingItem>();

            var createdList = await _fileDbService.CreateShoppingListAsync(list);

            return CreatedAtAction(nameof(GetList), new { listId = createdList.Id }, createdList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立購物清單失敗");
            return StatusCode(500, "建立購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 更新購物清單
    /// </summary>
    [HttpPut("{listId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingList>> UpdateList(string listId, [FromBody] ShoppingList list)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(listId))
            {
                return BadRequest("清單ID不能為空");
            }

            if (string.IsNullOrWhiteSpace(list.Title))
            {
                return BadRequest("清單標題不能為空");
            }

            var existingList = await _fileDbService.GetShoppingListById(listId);
            if (existingList == null)
            {
                return NotFound($"找不到ID為 {listId} 的購物清單");
            }

            // 更新清單資訊
            existingList.Title = list.Title;
            existingList.BuyDate = list.BuyDate;

            // 批量更新項目
            if (list.Items != null)
            {
                // 移除已刪除的項目
                var itemsToRemove = existingList.Items
                    .Where(existingItem => !list.Items.Any(newItem =>
                        !string.IsNullOrEmpty(newItem.Id) && newItem.Id == existingItem.Id))
                    .ToList();

                foreach (var itemToRemove in itemsToRemove)
                {
                    existingList.Items.Remove(itemToRemove);
                }

                // 更新或新增項目
                foreach (var newItem in list.Items)
                {
                    var existingItem = existingList.Items
                        .FirstOrDefault(item => !string.IsNullOrEmpty(newItem.Id) && item.Id == newItem.Id);

                    if (existingItem != null)
                    {
                        // 更新現有項目
                        existingItem.Name = newItem.Name;
                        existingItem.Quantity = newItem.Quantity;
                        existingItem.Price = newItem.Price;
                        existingItem.IsCompleted = newItem.IsCompleted;
                        existingItem.CompletedAt = newItem.IsCompleted ? DateTime.UtcNow : null;
                    }
                    else
                    {
                        // 新增項目
                        var itemToAdd = new ShoppingItem
                        {
                            Id = string.IsNullOrEmpty(newItem.Id) ? Guid.NewGuid().ToString() : newItem.Id,
                            Name = newItem.Name,
                            Quantity = newItem.Quantity,
                            Price = newItem.Price,
                            IsCompleted = newItem.IsCompleted,
                            CompletedAt = newItem.IsCompleted ? DateTime.UtcNow : null,
                            CreatedAt = DateTime.UtcNow
                        };
                        existingList.Items.Add(itemToAdd);
                    }
                }
            }

            // 儲存更新後的清單
            await _fileDbService.SaveShoppingList(existingList);

            return Ok(existingList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新購物清單 {ListId} 失敗", listId);
            return StatusCode(500, "更新購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 刪除購物清單
    /// </summary>
    [HttpDelete("{listId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteList(string listId)
    {
        try
        {
            var list = await _fileDbService.GetShoppingListById(listId);
            if (list == null)
            {
                return NotFound($"找不到ID為 {listId} 的購物清單");
            }

            await _fileDbService.DeleteShoppingList(listId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除購物清單 {ListId} 失敗", listId);
            return StatusCode(500, "刪除購物清單時發生錯誤");
        }
    }

    /// <summary>
    /// 匯出購物清單
    /// </summary>
    [HttpGet("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportLists()
    {
        try
        {
            var lists = await _fileDbService.GetAllShoppingListsAsync();
            var jsonString = JsonSerializer.Serialize(lists, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            return File(bytes, "application/json", $"shopping_lists_{DateTime.Now:yyyyMMddHHmmss}.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出購物清單失敗");
            return StatusCode(500, "匯出購物清單時發生錯誤");
        }
    }
}