using Microsoft.AspNetCore.Mvc;
using TodoTaskManagementAPI.Models;
using TodoTaskManagementAPI.Services;
using TodoTaskManagementAPI.Domain;
using Microsoft.AspNetCore.JsonPatch;

namespace TodoTaskManagementAPI.Controllers
{
    /// <summary>
    /// 待辦事項任務控制器，處理所有與任務相關的HTTP請求
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TodoTasksController : ControllerBase
    {
        // 任務服務實例，用於處理業務邏輯
        private readonly ITodoTaskService _todoTaskService;

        /// <summary>
        /// 控制器構造函數
        /// </summary>
        /// <param name="todoTaskService">任務服務依賴注入</param>
        public TodoTasksController(ITodoTaskService todoTaskService)
        {
            _todoTaskService = todoTaskService;
        }

        /// <summary>
        /// 獲取所有任務
        /// </summary>
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
                // 驗證分頁參數
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var result = await _todoTaskService.GetAllAsync(page, pageSize, isCompleted, priority);
                
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
        [HttpPost]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoTaskResponse>> CreateTask([FromBody] CreateTodoTaskRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var task = new TodoTask
                {
                    Title = request.Title,
                    Description = request.Description,
                    Priority = request.Priority,
                    DueDate = request.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    IsCompleted = false
                };

                var createdTask = await _todoTaskService.CreateTaskAsync(task);
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
        /// 根據ID獲取任務
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Domain.TodoTask), StatusCodes.Status200OK)]
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
        /// 更新任務
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoTaskResponse>> UpdateTask(int id, [FromBody] UpdateTodoTaskRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                task.Title = request.Title;
                task.Description = request.Description;
                task.Priority = request.Priority;
                task.DueDate = request.DueDate;
                task.UpdatedAt = DateTime.UtcNow;

                var updatedTask = await _todoTaskService.UpdateTaskAsync(task);
                return Ok(TodoTaskResponse.FromTodoTask(updatedTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"更新任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 刪除任務
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                await _todoTaskService.DeleteTaskAsync(task);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"刪除任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 完成任務
        /// </summary>
        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoTaskResponse>> CompleteTask(int id)
        {
            try
            {
                var task = await _todoTaskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"找不到ID為 {id} 的任務" });
                }

                task.IsCompleted = true;
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
        /// 重新打開任務
        /// </summary>
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
        /// 獲取逾期任務
        /// </summary>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(IEnumerable<TodoTaskResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoTaskResponse>>> GetOverdueTasks()
        {
            try
            {
                var tasks = await _todoTaskService.GetOverdueTasksAsync();
                return Ok(tasks.Select(TodoTaskResponse.FromTodoTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"獲取逾期任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 根據優先級獲取任務
        /// </summary>
        [HttpGet("priority/{priority}")]
        [ProducesResponseType(typeof(IEnumerable<TodoTaskResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoTaskResponse>>> GetTasksByPriority(Priority priority)
        {
            try
            {
                var tasks = await _todoTaskService.GetTasksByPriorityAsync(priority);
                return Ok(tasks.Select(TodoTaskResponse.FromTodoTask));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"根據優先級獲取任務時發生錯誤: {ex.Message}" });
            }
        }

        /// <summary>
        /// 更新任務狀態
        /// </summary>
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
    /// 更新任務狀態請求模型
    /// </summary>
    public class UpdateTaskStatusRequest
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
