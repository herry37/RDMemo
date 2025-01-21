using Microsoft.EntityFrameworkCore;
using TodoTaskManagementAPI.Domain;
using TodoTaskManagementAPI.Infrastructure;

namespace TodoTaskManagementAPI.Services
{
    /// <summary>
    /// 待辦事項任務服務實現
    /// </summary>
    public class TodoTaskService : ITodoTaskService
    {
        private readonly TodoTaskContext _context;

        public TodoTaskService(TodoTaskContext context)
        {
            _context = context;
        }

        public async Task<TodoTask> CreateTaskAsync(TodoTask task)
        {
            _context.TodoTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteTaskAsync(TodoTask task)
        {
            _context.TodoTasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<TodoTask?> GetTaskByIdAsync(int id)
        {
            return await _context.TodoTasks.FindAsync(id);
        }

        public async Task<PagedResult<TodoTask>> GetAllAsync(int page = 1, int pageSize = 10, bool? isCompleted = null, Priority? priority = null)
        {
            // 確保頁碼和頁面大小有效
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            // 構建查詢
            IQueryable<TodoTask> query = _context.TodoTasks;

            // 應用過濾條件
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            // 計算總記錄數和總頁數
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // 獲取當前頁的數據
            var items = await query
                .OrderByDescending(t => t.Priority)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TodoTask>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<TodoTask> UpdateTaskAsync(TodoTask task)
        {
            _context.TodoTasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<IEnumerable<TodoTask>> GetOverdueTasksAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.TodoTasks
                .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < now)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority)
        {
            return await _context.TodoTasks
                .Where(t => t.Priority == priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
