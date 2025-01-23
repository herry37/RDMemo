/// <summary>
/// 待辦事項管理系統的 API 控制器
/// 提供任務的增刪改查等 HTTP 接口
/// </summary>

using Microsoft.AspNetCore.Mvc;
using TodoTaskManagementAPI.Models;
using TodoTaskManagementAPI.Services;
using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Controllers
{
    /// <summary>
    /// 待辦事項任務控制器
    /// 處理所有與任務相關的 HTTP 請求，包括：
    /// 1. 任務的基本 CRUD 操作
    /// 2. 任務狀態管理
    /// 3. 任務篩選和分頁
    /// 4. 特殊任務查詢（如逾期任務）
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TodoTasksController : ControllerBase
    {
        /// <summary>
        /// 任務服務實例
        /// 通過依賴注入獲得，用於處理業務邏輯
        /// </summary>
        private readonly ITodoTaskService _todoTaskService;

        /// <summary>
        /// 控制器構造函數
        /// </summary>
        /// <param name="todoTaskService">任務服務實例，由依賴注入框架提供</param>
        /// <remarks>
        /// 使用依賴注入模式，降低組件間的耦合度
        /// 便於單元測試和維護
        /// </remarks>
        public TodoTasksController(ITodoTaskService todoTaskService)
        {
            _todoTaskService = todoTaskService;
        }

        /// <summary>
        /// 獲取任務列表
        /// 支持分頁、狀態篩選和優先級篩選
        /// </summary>
        /// <param name="page">頁碼，從1開始</param>
        /// <param name="pageSize">每頁數量，默認10條</param>
        /// <param name="isCompleted">完成狀態篩選</param>
        /// <param name="priority">優先級篩選</param>
        /// <returns>分頁後的任務列表</returns>
        /// <response code="200">成功獲取任務列表</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<TodoTaskResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponse<TodoTaskResponse>>> GetTasks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isCompleted = null,
            [FromQuery] Priority? priority = null)
        {
            try
            {
                // 驗證並調整分頁參數
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;  // 限制最大頁面大小

                // 獲取任務列表
                var result = await _todoTaskService.GetAllAsync(page, pageSize, isCompleted, priority);
                
                // 轉換為響應模型
                var response = new PaginatedResponse<TodoTaskResponse>
                {
                    Items = result.Items.Select(TodoTaskResponse.FromTodoTask).ToList(),
                    TotalItems = result.TotalItems,
                    CurrentPage = result.CurrentPage,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"獲取任務列表時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 創建新任務
        /// </summary>
        /// <param name="request">任務創建請求模型</param>
        /// <returns>創建成功的任務信息</returns>
        /// <remarks>
        /// 請求示例:
        /// POST /api/TodoTasks
        /// {
        ///     "title": "完成報告",
        ///     "description": "準備下週的工作報告",
        ///     "priority": 1,
        ///     "dueDate": "2024-01-31T12:00:00Z"
        /// }
        /// </remarks>
        /// <response code="201">任務創建成功</response>
        /// <response code="400">請求數據無效</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpPost]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoTaskResponse>> CreateTask([FromBody] CreateTodoTaskRequest request)
        {
            try
            {
                // 驗證請求模型
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 創建任務實體
                var task = new TodoTask
                {
                    Title = request.Title,
                    Description = request.Description,
                    Priority = request.Priority,
                    DueDate = request.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    IsCompleted = false
                };

                // 保存任務
                var createdTask = await _todoTaskService.CreateTaskAsync(task);
                
                // 返回創建成功的響應
                return CreatedAtAction(
                    nameof(GetTasks),
                    new { id = createdTask.Id },
                    TodoTaskResponse.FromTodoTask(createdTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"創建任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 根據ID獲取特定任務
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <returns>任務詳細信息</returns>
        /// <response code="200">成功獲取任務信息</response>
        /// <response code="404">指定ID的任務不存在</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                return Ok(TodoTaskResponse.FromTodoTask(task));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"獲取任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 更新現有任務
        /// </summary>
        /// <param name="id">要更新的任務ID</param>
        /// <param name="request">任務更新請求模型</param>
        /// <returns>更新後的任務信息</returns>
        /// <remarks>
        /// 請求示例:
        /// PUT /api/TodoTasks/1
        /// {
        ///     "title": "修改後的標題",
        ///     "description": "更新後的描述",
        ///     "priority": 2,
        ///     "dueDate": "2024-02-01T12:00:00Z"
        /// }
        /// </remarks>
        /// <response code="200">任務更新成功</response>
        /// <response code="400">請求數據無效</response>
        /// <response code="404">指定ID的任務不存在</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoTaskResponse>> UpdateTask(int id, [FromBody] UpdateTodoTaskRequest request)
        {
            try
            {
                // 驗證請求模型
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 檢查任務是否存在
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                // 更新任務屬性
                task.Title = request.Title;
                task.Description = request.Description;
                task.Priority = request.Priority;
                task.DueDate = request.DueDate;
                task.UpdatedAt = DateTime.UtcNow;

                // 保存更新
                var updatedTask = await _todoTaskService.UpdateTaskAsync(task);
                return Ok(TodoTaskResponse.FromTodoTask(updatedTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"更新任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 刪除指定任務
        /// </summary>
        /// <param name="id">要刪除的任務ID</param>
        /// <returns>無內容</returns>
        /// <response code="204">任務刪除成功</response>
        /// <response code="404">指定ID的任務不存在</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTask(int id)
        {
            try
            {
                // 檢查任務是否存在
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                // 執行刪除操作
                await _todoTaskService.DeleteTaskAsync(task);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"刪除任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 將任務標記為已完成
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <returns>更新後的任務信息</returns>
        /// <response code="200">任務狀態更新成功</response>
        /// <response code="404">指定ID的任務不存在</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoTaskResponse>> CompleteTask(int id)
        {
            try
            {
                // 檢查任務是否存在
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                // 更新任務狀態
                task.IsCompleted = true;
                task.CompletedAt = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;

                var updatedTask = await _todoTaskService.UpdateTaskAsync(task);
                return Ok(TodoTaskResponse.FromTodoTask(updatedTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"完成任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 重新打開已完成的任務
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <returns>更新後的任務信息</returns>
        /// <response code="200">任務重新打開成功</response>
        /// <response code="404">指定ID的任務不存在</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpPost("{id}/reopen")]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoTaskResponse>> ReopenTask(int id)
        {
            try
            {
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                task.IsCompleted = false;
                task.CompletedAt = null;
                task.UpdatedAt = DateTime.UtcNow;

                var updatedTask = await _todoTaskService.UpdateTaskAsync(task);
                return Ok(TodoTaskResponse.FromTodoTask(updatedTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"重新打開任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 更新任務狀態
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <param name="request">狀態更新請求</param>
        /// <returns>更新後的任務信息</returns>
        /// <response code="200">任務狀態更新成功</response>
        /// <response code="404">指定ID的任務不存在</response>
        /// <response code="500">服務器內部錯誤</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoTaskResponse>> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusRequest request)
        {
            try
            {
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                task.IsCompleted = request.IsCompleted;
                task.UpdatedAt = DateTime.UtcNow;
                if (request.IsCompleted)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
                else
                {
                    task.CompletedAt = null;
                }

                var updatedTask = await _todoTaskService.UpdateTaskAsync(task);
                return Ok(TodoTaskResponse.FromTodoTask(updatedTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"更新任務狀態時發生錯誤: {ex.Message}" });
            }
        }
    }

    /// <summary>
    /// 更新任務狀態的請求模型
    /// </summary>
    /// <remarks>
    /// 用於 PATCH 請求，僅更新任務的完成狀態
    /// </remarks>
    public class UpdateTaskStatusRequest
    {
        /// <summary>
        /// 任務是否完成
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
