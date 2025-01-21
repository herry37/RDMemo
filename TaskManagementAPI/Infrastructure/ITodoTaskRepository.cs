using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Infrastructure
{
    public interface ITodoTaskRepository
    {
        Task<TodoTask> GetByIdAsync(int id);
        
        Task<IEnumerable<TodoTask>> GetAllAsync(
            int? skip = null,
            int? take = null,
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);

        Task<int> GetTotalCountAsync(
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);

        Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();
        
        Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority);
        
        Task<TodoTask> AddAsync(TodoTask task);
        
        Task<TodoTask> UpdateAsync(TodoTask task);
        
        Task DeleteAsync(int id);
        
        Task<bool> ExistsAsync(int id);
    }
}
