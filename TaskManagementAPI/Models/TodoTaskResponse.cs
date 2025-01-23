/// <summary>
/// 任務管理 API 的響應模型定義
/// </summary>

using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 待辦事項任務的響應模型
    /// 用於向客戶端返回任務的完整信息
    /// </summary>
    /// <remarks>
    /// 此模型用於：
    /// 1. 封裝任務實體的公開數據
    /// 2. 提供一致的 API 響應格式
    /// 3. 隱藏內部實現細節
    /// 
    /// 特點：
    /// - 包含任務的所有基本屬性
    /// - 提供時間相關的審計信息
    /// - 支持從實體模型轉換
    /// </remarks>
    public class TodoTaskResponse
    {
        /// <summary>
        /// 任務唯一標識符
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 由數據庫自動生成
        /// 2. 在整個系統中唯一
        /// 3. 用於任務的檢索和引用
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// 任務標題
        /// 簡短描述任務的主要內容
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 必填字段
        /// 2. 用於列表顯示和快速識別
        /// 3. 最大長度為 200 個字符
        /// </remarks>
        public string Title { get; set; }

        /// <summary>
        /// 任務描述
        /// 詳細說明任務的具體內容、要求和注意事項
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 可選字段
        /// 2. 支持富文本或多行文本
        /// 3. 最大長度為 1000 個字符
        /// </remarks>
        public string? Description { get; set; }

        /// <summary>
        /// 任務完成狀態
        /// 標識任務是否已經完成
        /// </summary>
        /// <remarks>
        /// 用途：
        /// 1. 任務進度追蹤
        /// 2. 過濾和統計
        /// 3. UI 顯示狀態
        /// 
        /// 值含義：
        /// - true：任務已完成
        /// - false：任務未完成
        /// </remarks>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 任務優先級
        /// 指定任務的重要程度和緊急程度
        /// </summary>
        /// <remarks>
        /// 用途：
        /// 1. 任務排序
        /// 2. 視覺化顯示
        /// 3. 工作規劃
        /// 
        /// 可選值：
        /// - Low：低優先級任務
        /// - Medium：中優先級任務
        /// - High：高優先級任務
        /// </remarks>
        public Priority Priority { get; set; }

        /// <summary>
        /// 任務創建時間
        /// 記錄任務首次創建的時間戳
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 系統自動生成
        /// 2. 不可修改
        /// 3. 使用 UTC 時間存儲
        /// 
        /// 用途：
        /// - 任務時間線追蹤
        /// - 報表生成
        /// - 審計日誌
        /// </remarks>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 任務最後更新時間
        /// 記錄任務最近一次修改的時間戳
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 首次創建時為 null
        /// 2. 任何修改都會更新
        /// 3. 使用 UTC 時間存儲
        /// 
        /// 用途：
        /// - 變更追蹤
        /// - 版本控制
        /// - 數據同步
        /// </remarks>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 任務截止日期
        /// 指定任務需要完成的最後期限
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 可選字段
        /// 2. 支持日期和時間
        /// 3. 使用 UTC 時間存儲
        /// 
        /// 用途：
        /// - 任務規劃
        /// - 提醒通知
        /// - 逾期檢查
        /// </remarks>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// 從領域模型創建響應模型的工廠方法
        /// </summary>
        /// <param name="task">源任務實體對象</param>
        /// <returns>對應的響應模型實例</returns>
        /// <remarks>
        /// 功能：
        /// 1. 實現領域模型到 DTO 的轉換
        /// 2. 確保數據映射的一致性
        /// 3. 隱藏內部實現細節
        /// 
        /// 使用場景：
        /// - API 響應數據轉換
        /// - 批量數據轉換
        /// - 緩存數據準備
        /// </remarks>
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
