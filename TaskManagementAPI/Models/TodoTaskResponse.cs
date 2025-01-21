using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 任務響應模型
    /// </summary>
    public class TodoTaskResponse
    {
        /// <summary>
        /// 任務ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 任務標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 任務描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 優先級
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// 從 TodoTask 實體創建響應模型
        /// </summary>
        public static TodoTaskResponse FromTodoTask(TodoTask task)
        {
            return new TodoTaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                DueDate = task.DueDate
            };
        }
    }
}
