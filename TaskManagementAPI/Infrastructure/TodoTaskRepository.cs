using Microsoft.EntityFrameworkCore;
using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Infrastructure
{
    /// <summary>
    /// TodoTask 倉儲實現類，負責任務數據的持久化操作
    /// </summary>
    public class TodoTaskRepository : ITodoTaskRepository
    {
        private readonly TodoTaskContext _context;

        public TodoTaskRepository(TodoTaskContext context)
        {
            _context = context;
        }

        public async Task<TodoTask> GetByIdAsync(int id)
        {
            return await _context.TodoTasks.FindAsync(id);
        }

        public async Task<IEnumerable<TodoTask>> GetAllAsync(
            int? skip = null,
            int? take = null,
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null)
        {
            var query = _context.TodoTasks.AsQueryable();

            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            if (dueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= dueDateFrom.Value);
            }

            if (dueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= dueDateTo.Value);
            }

            query = query.OrderByDescending(t => t.Priority)
                        .ThenByDescending(t => t.CreatedAt);

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null)
        {
            var query = _context.TodoTasks.AsQueryable();

            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            if (dueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= dueDateFrom.Value);
            }

            if (dueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= dueDateTo.Value);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetOverdueTasksAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.TodoTasks
                .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority)
        {
            return await _context.TodoTasks
                .Where(t => t.Priority == priority && !t.IsCompleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TodoTask> AddAsync(TodoTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _context.TodoTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TodoTask> UpdateAsync(TodoTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var entry = _context.Entry(task);
            if (entry.State == EntityState.Detached)
            {
                _context.TodoTasks.Attach(task);
            }
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteAsync(int id)
        {
            var task = await GetByIdAsync(id);
            if (task != null)
            {
                _context.TodoTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TodoTasks
                .AsNoTracking()
                .AnyAsync(t => t.Id == id);
        }
    }
}
