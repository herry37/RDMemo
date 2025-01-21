using TodoTaskManagementAPI.Domain;
using TodoTaskManagementAPI.Infrastructure;

namespace TodoTaskManagementAPI.Services
{
    /// <summary>
    /// 待辦事項任務服務接口
    /// </summary>
    public interface ITodoTaskService
    {
        /// <summary>
        /// 建立新任務
        /// </summary>
        Task<TodoTask> CreateTaskAsync(TodoTask task);

        /// <summary>
        /// 根據ID獲取任務
        /// </summary>
        Task<TodoTask?> GetTaskByIdAsync(int id);

        /// <summary>
        /// 獲取所有任務
        /// </summary>
        Task<PagedResult<TodoTask>> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            bool? isCompleted = null,
            Priority? priority = null);

        /// <summary>
        /// 更新任務
        /// </summary>
        Task<TodoTask> UpdateTaskAsync(TodoTask task);

        /// <summary>
        /// 刪除任務
        /// </summary>
        Task DeleteTaskAsync(TodoTask task);

        /// <summary>
        /// 獲取逾期任務
        /// </summary>
        Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();

        /// <summary>
        /// 根據優先級獲取任務
        /// </summary>
        Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority);
    }
}
