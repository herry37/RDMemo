/// <summary>
/// 待辦事項管理系統的領域模型
/// 包含任務實體和相關枚舉的定義
/// </summary>

// 引入必要的命名空間，用於數據驗證和數據庫映射
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoTaskManagementAPI.Domain
{
    /// <summary>
    /// 待辦事項任務實體類別
    /// 代表系統中的一個具體任務，包含任務的所有屬性和行為
    /// </summary>
    /// <remarks>
    /// 此類別用於：
    /// 1. 定義任務的數據結構
    /// 2. 提供任務的業務邏輯
    /// 3. 與數據庫表映射
    /// 4. 提供數據驗證規則
    /// </remarks>
    [Table("Tasks")] // 指定數據庫表名為 "Tasks"
    public class TodoTask
    {
        /// <summary>
        /// 任務的唯一識別碼
        /// 由數據庫自動生成
        /// </summary>
        /// <remarks>
        /// - 主鍵
        /// - 自增長
        /// - 不可為空
        /// </remarks>
        [Key] // 標記為主鍵
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 設置為自增長
        public int Id { get; set; } // 任務ID，唯一標識每個任務
        
        /// <summary>
        /// 任務標題
        /// 簡短描述任務的主要內容
        /// </summary>
        /// <remarks>
        /// 限制：
        /// - 必填
        /// - 最大長度 200 字符
        /// - 不允許為 null 或空字符串
        /// </remarks>
        [Required] // 標記為必填欄位
        [MaxLength(200)] // 限制最大長度為 200
        public string Title { get; set; } = string.Empty; // 任務標題，初始化為空字符串
        
        /// <summary>
        /// 任務詳細描述
        /// 提供關於任務的詳細信息
        /// </summary>
        /// <remarks>
        /// 限制：
        /// - 可選
        /// - 最大長度 1000 字符
        /// - 默認為空字符串
        /// </remarks>
        [MaxLength(1000)] // 限制最大長度為 1000
        public string Description { get; set; } = string.Empty; // 任務描述，初始化為空字符串
        
        /// <summary>
        /// 任務創建時間
        /// 記錄任務被創建的 UTC 時間
        /// </summary>
        /// <remarks>
        /// - 自動設置為當前 UTC 時間
        /// - 不可修改
        /// - 用於追蹤任務生命週期
        /// </remarks>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 創建時間，默認為當前 UTC 時間
        
        /// <summary>
        /// 任務最後更新時間
        /// 記錄任務最後一次被修改的時間
        /// </summary>
        /// <remarks>
        /// - 可為空（表示從未更新）
        /// - 每次更新任務時自動更新
        /// - 使用 UTC 時間
        /// </remarks>
        public DateTime? UpdatedAt { get; set; } // 更新時間，可為空
        
        /// <summary>
        /// 任務完成時間
        /// 記錄任務被標記為完成的時間
        /// </summary>
        /// <remarks>
        /// - 可為空（表示尚未完成）
        /// - 當任務完成時自動設置
        /// - 重新打開任務時自動清除
        /// </remarks>
        [Column("CompleteGdt")] // 映射到數據庫中的 CompleteGdt 列
        public DateTime? CompletedAt { get; set; } // 完成時間，可為空
        
        /// <summary>
        /// 任務截止日期
        /// 指定任務應該完成的目標日期
        /// </summary>
        /// <remarks>
        /// - 可選
        /// - 用於任務管理和提醒
        /// - 可用於識別逾期任務
        /// </remarks>
        public DateTime? DueDate { get; set; } // 截止日期，可為空
        
        /// <summary>
        /// 任務是否已完成
        /// 標識任務是否已經完成
        /// </summary>
        /// <remarks>
        /// - 默認為 false（未完成）
        /// - 可通過 API 更新
        /// - 更新時會同步設置 CompletedAt
        /// </remarks>
        public bool IsCompleted { get; set; } // 完成狀態，布爾值
        
        /// <summary>
        /// 任務優先級
        /// 表示任務的重要程度
        /// </summary>
        /// <remarks>
        /// 限制：
        /// - 必填
        /// - 存儲為整數
        /// - 默認為低優先級
        /// - 用於任務排序和過濾
        /// </remarks>
        [Column(TypeName = "int")] // 在數據庫中存儲為整數
        [Required] // 標記為必填欄位
        public Priority Priority { get; set; } = Priority.Low; // 優先級，默認為低優先級

        /// <summary>
        /// 默認建構函數
        /// 用於 Entity Framework Core 的實體創建
        /// </summary>
        /// <remarks>
        /// - 不應直接使用此構造函數
        /// - 僅供框架內部使用
        /// - 所有屬性使用默認值
        /// </remarks>
        public TodoTask() // 默認構造函數，供 EF Core 使用
        {
        }

        /// <summary>
        /// 創建新的待辦事項任務
        /// 提供便捷的任務創建方法
        /// </summary>
        /// <param name="title">任務標題，必填且不能為空</param>
        /// <param name="description">任務描述，可選</param>
        /// <param name="dueDate">截止日期，可選</param>
        /// <param name="priority">優先級，預設為低優先</param>
        /// <remarks>
        /// 創建規則：
        /// 1. 標題為必填項且不能為空
        /// 2. 自動設置創建時間和更新時間
        /// 3. 初始狀態為未完成
        /// 4. 所有時間使用 UTC 時間儲存
        /// </remarks>
        /// <exception cref="ArgumentException">當標題為空或空白時拋出</exception>
        public TodoTask(string title, string? description = null, DateTime? dueDate = null, Priority priority = Priority.Low)
        {
            // 驗證標題不能為空或空白
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("任務標題不能為空", nameof(title));

            // 初始化任務屬性
            Title = title; // 設置任務標題
            Description = description ?? string.Empty; // 設置任務描述，如果為 null 則使用空字符串
            DueDate = dueDate; // 設置截止日期
            Priority = priority; // 設置優先級
            CreatedAt = DateTime.UtcNow; // 設置創建時間為當前 UTC 時間
            UpdatedAt = DateTime.UtcNow; // 設置更新時間為當前 UTC 時間
            IsCompleted = false; // 初始狀態為未完成
        }

        /// <summary>
        /// 用於數據庫種子數據的內部構造函數
        /// 允許指定所有屬性，包括 ID
        /// </summary>
        /// <remarks>
        /// 使用限制：
        /// - 僅用於數據庫初始化
        /// - 僅供內部使用
        /// - 不應在應用程序代碼中直接使用
        /// </remarks>
        internal static TodoTask CreateForSeed(int id, string title, string description, DateTime? dueDate, Priority priority)
        {
            // 創建用於數據庫種子數據的任務實例
            return new TodoTask
            {
                Id = id, // 設置指定的 ID
                Title = title, // 設置標題
                Description = description, // 設置描述
                DueDate = dueDate, // 設置截止日期
                Priority = priority, // 設置優先級
                CreatedAt = DateTime.UtcNow, // 設置創建時間
                UpdatedAt = DateTime.UtcNow, // 設置更新時間
                IsCompleted = false // 設置為未完成狀態
            };
        }

        /// <summary>
        /// 檢查任務是否已逾期
        /// </summary>
        /// <returns>如果任務已逾期則返回 true，否則返回 false</returns>
        /// <remarks>
        /// 判斷規則：
        /// 1. 已完成的任務永遠不算逾期
        /// 2. 沒有截止日期的任務永遠不算逾期
        /// 3. 當前時間超過截止日期則算逾期
        /// </remarks>
        public bool IsOverdue()
        {
            // 如果任務已完成或沒有截止日期，則不算逾期
            if (IsCompleted || !DueDate.HasValue)
                return false;

            // 比較當前時間和截止日期
            return DateTime.UtcNow > DueDate.Value;
        }

        /// <summary>
        /// 將任務標記為已完成
        /// </summary>
        /// <remarks>
        /// - 如果任務尚未完成，則標記為完成並記錄完成時間
        /// - 如果任務已完成，則不進行任何操作
        /// </remarks>
        public void Complete()
        {
            // 如果任務尚未完成，則進行完成操作
            if (!IsCompleted)
            {
                IsCompleted = true; // 設置完成狀態
                CompletedAt = DateTime.UtcNow; // 記錄完成時間
                UpdatedAt = DateTime.UtcNow; // 更新修改時間
            }
        }

        /// <summary>
        /// 重新開啟已完成的任務
        /// </summary>
        /// <remarks>
        /// - 如果任務已完成，則重新開啟並清除完成時間
        /// - 如果任務尚未完成，則不進行任何操作
        /// </remarks>
        public void Reopen()
        {
            // 如果任務已完成，則進行重新開啟操作
            if (IsCompleted)
            {
                IsCompleted = false; // 設置為未完成狀態
                CompletedAt = null; // 清除完成時間
                UpdatedAt = DateTime.UtcNow; // 更新修改時間
            }
        }

        /// <summary>
        /// 更新任務標題
        /// </summary>
        /// <param name="newTitle">新的任務標題</param>
        /// <remarks>
        /// - 驗證新標題不能為空或空白
        /// - 更新標題後自動更新更新時間
        /// </remarks>
        /// <exception cref="ArgumentException">當新標題為空或空白時拋出</exception>
        public void UpdateTitle(string newTitle)
        {
            // 驗證新標題不能為空或空白
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("任務標題不能為空", nameof(newTitle));

            Title = newTitle; // 更新標題
            UpdatedAt = DateTime.UtcNow; // 更新修改時間
        }

        /// <summary>
        /// 更新任務描述
        /// </summary>
        /// <param name="newDescription">新的任務描述</param>
        /// <remarks>
        /// - 可選更新描述
        /// - 更新描述後自動更新更新時間
        /// </remarks>
        public void UpdateDescription(string? newDescription)
        {
            Description = newDescription ?? string.Empty; // 更新描述，如果為 null 則使用空字符串
            UpdatedAt = DateTime.UtcNow; // 更新修改時間
        }

        /// <summary>
        /// 更新任務截止日期
        /// </summary>
        /// <param name="newDueDate">新的截止日期</param>
        /// <remarks>
        /// - 可選更新截止日期
        /// - 更新截止日期後自動更新更新時間
        /// </remarks>
        public void UpdateDueDate(DateTime? newDueDate)
        {
            DueDate = newDueDate; // 更新截止日期
            UpdatedAt = DateTime.UtcNow; // 更新修改時間
        }

        /// <summary>
        /// 更新任務優先級
        /// </summary>
        /// <param name="newPriority">新的優先級</param>
        /// <remarks>
        /// - 必填更新優先級
        /// - 更新優先級後自動更新更新時間
        /// </remarks>
        public void UpdatePriority(Priority newPriority)
        {
            Priority = newPriority; // 更新優先級
            UpdatedAt = DateTime.UtcNow; // 更新修改時間
        }
    }

    /// <summary>
    /// 任務優先級枚舉
    /// 定義任務的重要程度等級
    /// </summary>
    /// <remarks>
    /// 優先級說明：
    /// - Low (0)：低優先級，可以稍後處理的任務
    /// - Medium (1)：中優先級，需要適時處理的任務
    /// - High (2)：高優先級，需要優先處理的任務
    /// - Urgent (3)：緊急優先級，需要立即處理的任務
    /// 
    /// 使用場景：
    /// 1. 任務排序
    /// 2. 任務過濾
    /// 3. 任務分類顯示
    /// </remarks>
    public enum Priority // 優先級枚舉，用於定義任務的重要程度
    {
        /// <summary>
        /// 低優先級
        /// 表示可以延後處理的任務
        /// </summary>
        Low = 0, // 最低優先級，值為 0

        /// <summary>
        /// 中優先級
        /// 表示需要在適當時間內處理的任務
        /// </summary>
        Medium = 1, // 中等優先級，值為 1

        /// <summary>
        /// 高優先級
        /// 表示需要優先處理的任務
        /// </summary>
        High = 2, // 高優先級，值為 2

        /// <summary>
        /// 緊急優先級
        /// 表示需要立即處理的任務
        /// </summary>
        Urgent = 3 // 緊急優先級，值為 3
    }
}
