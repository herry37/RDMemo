using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoTaskManagementAPI.Domain
{
    /// <summary>
    /// 待辦事項任務類別，代表系統中的一個任務實體
    /// </summary>
    [Table("Tasks")]
    public class TodoTask
    {
        /// <summary>
        /// 任務的唯一識別碼
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// 任務標題
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// 任務詳細描述
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 任務創建時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 任務最後更新時間
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// 任務截止日期
        /// </summary>
        public DateTime? DueDate { get; set; }
        
        /// <summary>
        /// 任務是否已完成
        /// </summary>
        public bool IsCompleted { get; set; }
        
        /// <summary>
        /// 任務優先級
        /// </summary>
        [Column(TypeName = "int")]
        [Required]
        public Priority Priority { get; set; } = Priority.Low;

        /// <summary>
        /// 建構函數
        /// </summary>
        public TodoTask()
        {
        }

        /// <summary>
        /// 創建新的待辦事項任務
        /// </summary>
        /// <param name="title">任務標題，不能為空</param>
        /// <param name="description">任務描述，可選</param>
        /// <param name="dueDate">截止日期，可選</param>
        /// <param name="priority">優先級，預設為低優先</param>
        public TodoTask(string title, string? description = null, DateTime? dueDate = null, Priority priority = Priority.Low)
        {
            // 驗證標題不能為空或空白
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("任務標題不能為空", nameof(title));

            // 初始化任務屬性
            Title = title;
            Description = description ?? string.Empty;
            DueDate = dueDate;
            Priority = priority;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsCompleted = false;
        }

        /// <summary>
        /// 用於數據庫種子數據的內部構造函數
        /// </summary>
        internal static TodoTask CreateForSeed(int id, string title, string description, DateTime? dueDate, Priority priority)
        {
            var task = new TodoTask
            {
                Id = id,
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = priority,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsCompleted = false
            };
            return task;
        }

        /// <summary>
        /// 將任務標記為已完成
        /// </summary>
        public void Complete()
        {
            // 如果任務尚未完成，則標記為完成並記錄完成時間
            if (!IsCompleted)
            {
                IsCompleted = true;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// 重新開啟已完成的任務
        /// </summary>
        public void Reopen()
        {
            // 如果任務已完成，則重新開啟並清除完成時間
            if (IsCompleted)
            {
                IsCompleted = false;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// 更新任務標題
        /// </summary>
        /// <param name="newTitle">新的任務標題</param>
        public void UpdateTitle(string newTitle)
        {
            // 驗證新標題不能為空或空白
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("任務標題不能為空", nameof(newTitle));

            Title = newTitle;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新任務描述
        /// </summary>
        /// <param name="newDescription">新的任務描述</param>
        public void UpdateDescription(string? newDescription)
        {
            Description = newDescription ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新任務截止日期
        /// </summary>
        /// <param name="newDueDate">新的截止日期</param>
        public void UpdateDueDate(DateTime? newDueDate)
        {
            DueDate = newDueDate;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新任務優先級
        /// </summary>
        /// <param name="newPriority">新的優先級</param>
        public void UpdatePriority(Priority newPriority)
        {
            Priority = newPriority;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 檢查任務是否已過期
        /// </summary>
        [NotMapped]
        public bool IsOverdue => !IsCompleted && DueDate.HasValue && DueDate.Value < DateTime.UtcNow;
    }

    /// <summary>
    /// 任務優先級枚舉
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// 低優先級
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "低優先級")]
        Low = 0,

        /// <summary>
        /// 中等優先級
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "中優先級")]
        Medium = 1,

        /// <summary>
        /// 高優先級
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "高優先級")]
        High = 2,

        /// <summary>
        /// 緊急優先級
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "緊急優先級")]
        Urgent = 3
    }
}
