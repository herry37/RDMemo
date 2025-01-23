using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Infrastructure
{
    /// <summary>
    /// 任務存儲庫介面
    /// </summary>
    public interface ITodoTaskRepository
    {
        /// <summary>
        /// 根據ID獲取特定任務
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <returns>獲取的任務</returns>
        Task<TodoTask> GetByIdAsync(int id);

        /// <summary>
        /// 異步獲取任務列表
        /// </summary>
        /// <param name="skip">跳過的記錄數</param>
        /// <param name="take">獲取的記錄數</param>
        /// <param name="isCompleted">完成狀態過濾</param>
        /// <param name="priority">優先級過濾</param>
        /// <param name="dueDateFrom">截止日期起始</param>
        /// <param name="dueDateTo">截止日期結束</param>
        /// <returns>符合條件的任務列表</returns>
        Task<IEnumerable<TodoTask>> GetAllAsync(
            int? skip = null,
            int? take = null,
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);

        /// <summary>
        /// 異步獲取符合條件的任務總數
        /// </summary>
        /// <param name="isCompleted">完成狀態過濾</param>
        /// <param name="priority">優先級過濾</param>
        /// <param name="dueDateFrom">截止日期起始</param>
        /// <param name="dueDateTo">截止日期結束</param>
        /// <returns>符合條件的記錄總數</returns>
        Task<int> GetTotalCountAsync(
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);

        /// <summary>
        /// 異步獲取逾期任務
        /// </summary>
        /// <returns>逾期任務列表</returns>
        Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();

        /// <summary>
        /// 根據優先級獲取任務列表
        /// </summary>
        /// <param name="priority">優先級</param>
        /// <returns>獲取的任務列表</returns>
        Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority);

        /// <summary>
        /// 異步添加新任務
        /// </summary>
        /// <param name="task">要添加的任務</param>
        /// <returns>添加成功的任務</returns>
        Task<TodoTask> AddAsync(TodoTask task);

        /// <summary>
        /// 異步更新現有任務
        /// </summary>
        /// <param name="task">要更新的任務</param>
        /// <returns>更新後的任務</returns>
        Task<TodoTask> UpdateAsync(TodoTask task);

        /// <summary>
        /// 異步刪除特定任務
        /// </summary>
        /// <param name="id">要刪除的任務ID</param>
        /// <returns>是否删除成功</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 檢查任務是否存在
        /// </summary>
        /// <param name="id">要檢查的任務ID</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(int id);
    }
}
