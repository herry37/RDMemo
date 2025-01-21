namespace TodoTaskManagementAPI.Domain
{
    /// <summary>
    /// 待辦事項任務倉儲介面，定義了所有任務相關的數據操作方法
    /// </summary>
    public interface ITodoTaskRepository
    {
        /// <summary>
        /// 根據ID異步獲取單個任務
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <returns>返回指定ID的任務，如果不存在則返回null</returns>
        Task<TodoTask> GetByIdAsync(int id);

        /// <summary>
        /// 異步獲取任務列表，支持分頁和多種篩選條件
        /// </summary>
        /// <param name="skip">跳過的記錄數，用於分頁</param>
        /// <param name="take">獲取的記錄數，用於分頁</param>
        /// <param name="isCompleted">是否已完成的篩選條件</param>
        /// <param name="priority">優先級篩選條件</param>
        /// <param name="dueDateFrom">截止日期範圍起始時間</param>
        /// <param name="dueDateTo">截止日期範圍結束時間</param>
        /// <returns>返回符合條件的任務列表</returns>
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
        /// <param name="isCompleted">是否已完成的篩選條件</param>
        /// <param name="priority">優先級篩選條件</param>
        /// <param name="dueDateFrom">截止日期範圍起始時間</param>
        /// <param name="dueDateTo">截止日期範圍結束時間</param>
        /// <returns>返回符合條件的任務總數</returns>
        Task<int> GetTotalCountAsync(
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);
            
        /// <summary>
        /// 異步獲取所有已過期的任務
        /// </summary>
        /// <returns>返回所有已過期但未完成的任務列表</returns>
        Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();

        /// <summary>
        /// 異步獲取指定優先級的所有任務
        /// </summary>
        /// <param name="priority">要查詢的優先級</param>
        /// <returns>返回指定優先級的任務列表</returns>
        Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority);

        /// <summary>
        /// 異步添加新任務
        /// </summary>
        /// <param name="task">要添加的任務實體</param>
        /// <returns>返回添加後的任務實體（包含生成的ID）</returns>
        Task<TodoTask> AddAsync(TodoTask task);

        /// <summary>
        /// 異步更新現有任務
        /// </summary>
        /// <param name="task">要更新的任務實體</param>
        /// <returns>更新操作的異步任務</returns>
        Task UpdateAsync(TodoTask task);

        /// <summary>
        /// 異步刪除指定ID的任務
        /// </summary>
        /// <param name="id">要刪除的任務ID</param>
        /// <returns>刪除操作的異步任務</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 異步檢查指定ID的任務是否存在
        /// </summary>
        /// <param name="id">要檢查的任務ID</param>
        /// <returns>如果任務存在則返回true，否則返回false</returns>
        Task<bool> ExistsAsync(int id);
    }
}
