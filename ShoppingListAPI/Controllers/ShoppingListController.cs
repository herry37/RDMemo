using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services.FileDb;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingListController : ControllerBase
    {
        private readonly ILogger<ShoppingListController> _logger;
        private readonly IFileDbService _fileDbService;

        public ShoppingListController(
            ILogger<ShoppingListController> logger,
            IFileDbService fileDbService)
        {
            _logger = logger;
            _fileDbService = fileDbService;
        }

        /// <summary>
        /// 取得所有購物清單
        /// </summary>
        /// <returns>購物清單集合</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllLists()
        {
            try
            {
                _logger.LogInformation("開始取得所有購物清單");
                var lists = await _fileDbService.GetAllShoppingListsAsync();

                // 確保回傳的資料格式正確
                var result = new
                {
                    success = true,
                    data = lists.Select(list =>
                    {
                        // 確保所有必要欄位都有值
                        list.Items ??= new List<ShoppingItem>();
                        foreach (var item in list.Items)
                        {
                            item.Quantity = Math.Max(1, item.Quantity);
                            item.Price ??= 0;
                        }

                        // 重新計算總金額
                        list.totalAmount = list.Items.Sum(item =>
                            (item.Price ?? 0) * Math.Max(1, item.Quantity));

                        return list;
                    }).OrderByDescending(x => x.BuyDate)
                };

                _logger.LogInformation("成功取得 {Count} 個購物清單", lists.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得購物清單時發生錯誤");
                return StatusCode(500, new { success = false, message = "取得購物清單時發生錯誤" });
            }
        }

        /// <summary>
        /// 根據 ID 取得購物清單
        /// </summary>
        /// <param name="id">購物清單 ID</param>
        /// <returns>購物清單詳細資料</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetListById(string id)
        {
            try
            {
                _logger.LogInformation("開始取得購物清單，ID: {Id}", id);

                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "購物清單 ID 不能為空"
                    });
                }

                var list = await _fileDbService.GetShoppingListAsync(id);
                if (list == null)
                {
                    _logger.LogWarning("找不到指定的購物清單，ID: {Id}", id);
                    return NotFound(new
                    {
                        success = false,
                        message = "找不到指定的購物清單"
                    });
                }

                _logger.LogInformation("成功取得購物清單，ID: {Id}", id);
                return Ok(new
                {
                    success = true,
                    message = "成功取得購物清單",
                    data = list
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得購物清單時發生錯誤，ID: {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "取得購物清單時發生錯誤",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 搜尋購物清單
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="title">標題關鍵字</param>
        /// <returns>符合條件的購物清單集合</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchLists(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? title = null)
        {
            try
            {
                _logger.LogInformation("搜尋購物清單 - 開始日期: {StartDate}, 結束日期: {EndDate}, 標題: {Title}",
                    startDate?.ToString("yyyy-MM-dd"),
                    endDate?.ToString("yyyy-MM-dd"),
                    title);

                var lists = await _fileDbService.GetAllShoppingListsAsync();
                if (lists == null || !lists.Any())
                {
                    _logger.LogInformation("未找到任何購物清單");
                    return Ok(new { success = true, data = new List<ShoppingList>() });
                }

                var query = lists.AsEnumerable();

                // 篩選日期範圍 (只比較日期部分)
                if (startDate.HasValue)
                {
                    var startDateOnly = DateOnly.FromDateTime(startDate.Value.Date);
                    query = query.Where(l => DateOnly.FromDateTime(l.BuyDate.Date) >= startDateOnly);
                }

                if (endDate.HasValue)
                {
                    var endDateOnly = DateOnly.FromDateTime(endDate.Value.Date);
                    query = query.Where(l => DateOnly.FromDateTime(l.BuyDate.Date) <= endDateOnly);
                }

                // 篩選標題 (不區分大小寫)
                if (!string.IsNullOrWhiteSpace(title))
                {
                    var searchTitle = title.Trim();
                    query = query.Where(l => l.Title != null && 
                        l.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase));
                }

                // 依購買日期降序排序
                var result = query
                    .OrderByDescending(l => l.BuyDate.Date)
                    .ToList();

                _logger.LogInformation("搜尋完成，找到 {Count} 筆符合條件的清單", result.Count);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜尋購物清單時發生錯誤");
                return StatusCode(500, new { success = false, message = "搜尋購物清單時發生錯誤" });
            }
        }

        /// <summary>
        /// 建立新的購物清單
        /// </summary>
        /// <param name="request">新增請求資料</param>
        /// <returns>新建立的購物清單</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateList([FromBody] ShoppingListCreateRequest request)
        {
            try
            {
                _logger.LogInformation("開始建立新的購物清單");

                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "請求資料不能為空"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "標題不能為空"
                    });
                }

                var newList = new ShoppingList
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = request.Title.Trim(),
                    BuyDate = request.BuyDate,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Items = new List<ShoppingItem>()
                };

                await _fileDbService.CreateShoppingListAsync(newList);

                _logger.LogInformation("成功建立新的購物清單，ID: {Id}", newList.Id);

                return Ok(new
                {
                    success = true,
                    message = "成功建立購物清單",
                    data = newList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "建立購物清單時發生錯誤");
                return StatusCode(500, new
                {
                    success = false,
                    message = "建立購物清單時發生錯誤",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 更新購物清單
        /// </summary>
        /// <param name="id">購物清單 ID</param>
        /// <param name="request">更新請求資料</param>
        /// <returns>更新後的購物清單</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateList(string id, [FromBody] ShoppingListUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("開始更新購物清單，ID: {Id}", id);

                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "購物清單 ID 不能為空"
                    });
                }

                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "更新資料不能為空"
                    });
                }

                // 檢查清單是否存在
                var existingList = await _fileDbService.GetShoppingListAsync(id);
                if (existingList == null)
                {
                    _logger.LogWarning("找不到指定的購物清單，ID: {Id}", id);
                    return NotFound(new
                    {
                        success = false,
                        message = "找不到指定的購物清單"
                    });
                }

                // 更新清單內容
                existingList.Title = request.Title;
                existingList.BuyDate = request.BuyDate;
                existingList.Items = request.Items ?? new List<ShoppingItem>();
                existingList.UpdatedAt = DateTime.Now;

                // 驗證項目
                foreach (var item in existingList.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.Name))
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "商品名稱不能為空白"
                        });
                    }

                    if (item.Quantity < 1)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "商品數量必須大於 0"
                        });
                    }

                    if (item.Price < 0)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "商品價格不能小於 0"
                        });
                    }
                }

                // 更新清單
                var updatedList = await _fileDbService.UpdateShoppingListAsync(existingList);

                _logger.LogInformation("成功更新購物清單，ID: {Id}", id);
                return Ok(new
                {
                    success = true,
                    message = "成功更新購物清單",
                    data = updatedList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新購物清單時發生錯誤，ID: {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "更新購物清單時發生錯誤",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 刪除購物清單
        /// </summary>
        /// <param name="id">購物清單 ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteList(string id)
        {
            try
            {
                _logger.LogInformation("開始刪除購物清單，ID: {Id}", id);

                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "購物清單 ID 不能為空"
                    });
                }

                // 檢查清單是否存在
                var existingList = await _fileDbService.GetShoppingListAsync(id);
                if (existingList == null)
                {
                    _logger.LogWarning("找不到指定的購物清單，ID: {Id}", id);
                    return NotFound(new
                    {
                        success = false,
                        message = "找不到指定的購物清單"
                    });
                }

                // 刪除清單
                await _fileDbService.DeleteShoppingList(id);

                // 重新取得所有清單，確保資料一致性
                var lists = await _fileDbService.GetAllShoppingListsAsync();

                _logger.LogInformation("成功刪除購物清單，ID: {Id}", id);
                return Ok(new
                {
                    success = true,
                    message = "成功刪除購物清單",
                    data = lists
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除購物清單時發生錯誤，ID: {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "刪除購物清單時發生錯誤",
                    error = ex.Message
                });
            }
        }
    }
}