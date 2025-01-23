namespace TodoTaskManagementAPI.Domain
{
    /// <summary>
    /// 待辦事項任務倉儲介面
    /// 定義了所有任務相關的數據操作方法
    /// </summary>
    /// <remarks>
    /// 此介面提供：
    /// 1. 基本的 CRUD 操作（創建、讀取、更新、刪除）
    /// 2. 高級查詢功能（分頁、篩選）
    /// 3. 特定業務邏輯查詢（過期任務、優先級任務）
    /// 
    /// 實現此介面時應注意：
    /// - 所有方法都是異步的，返回 Task 或 Task{T}
    /// - 查詢方法支持多種篩選條件組合
    /// - 需要處理並發訪問的情況
    /// - 應實現適當的錯誤處理機制
    /// </remarks>
    public interface ITodoTaskRepository
    {
        /// <summary>
        /// 根據ID異步獲取單個任務
        /// </summary>
        /// <param name="id">任務ID，必須大於0</param>
        /// <returns>返回指定ID的任務，如果不存在則返回null</returns>
        /// <remarks>
        /// 使用場景：
        /// - 查看任務詳情
        /// - 更新任務前的數據獲取
        /// - 驗證任務是否存在
        /// 
        /// 注意事項：
        /// - 應處理無效的ID輸入
        /// - 當任務不存在時返回null而不是拋出異常
        /// </remarks>
        Task<TodoTask> GetByIdAsync(int id);

        /// <summary>
        /// 異步獲取任務列表
        /// 支持分頁和多種篩選條件的組合查詢
        /// </summary>
        /// <param name="skip">跳過的記錄數，用於分頁，可選</param>
        /// <param name="take">獲取的記錄數，用於分頁，可選</param>
        /// <param name="isCompleted">是否已完成的篩選條件，可選</param>
        /// <param name="priority">優先級篩選條件，可選</param>
        /// <param name="dueDateFrom">截止日期範圍起始時間，可選</param>
        /// <param name="dueDateTo">截止日期範圍結束時間，可選</param>
        /// <returns>返回符合條件的任務列表</returns>
        /// <remarks>
        /// 查詢規則：
        /// 1. 所有參數都是可選的，未指定時不進行相應的篩選
        /// 2. 日期範圍查詢是閉區間，包含起始和結束日期
        /// 3. 結果應按創建時間降序排序
        /// 
        /// 分頁說明：
        /// - skip：從結果集中跳過的記錄數，用於實現分頁
        /// - take：要返回的記錄數量，用於控制每頁大小
        /// 
        /// 使用場景：
        /// - 任務列表頁面的數據獲取
        /// - 根據不同條件篩選任務
        /// - 實現分頁顯示
        /// </remarks>
        Task<IEnumerable<TodoTask>> GetAllAsync(
            int? skip = null,
            int? take = null,
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);
        
        /// <summary>
        /// 異步獲取符合條件的任務總數
        /// 用於分頁和統計
        /// </summary>
        /// <param name="isCompleted">是否已完成的篩選條件，可選</param>
        /// <param name="priority">優先級篩選條件，可選</param>
        /// <param name="dueDateFrom">截止日期範圍起始時間，可選</param>
        /// <param name="dueDateTo">截止日期範圍結束時間，可選</param>
        /// <returns>返回符合條件的任務總數</returns>
        /// <remarks>
        /// 使用場景：
        /// 1. 計算分頁總頁數
        /// 2. 顯示任務統計信息
        /// 3. 生成報表數據
        /// 
        /// 查詢規則：
        /// - 參數規則與 GetAllAsync 相同
        /// - 不考慮分頁參數
        /// - 只返回符合條件的記錄總數
        /// </remarks>
        Task<int> GetTotalCountAsync(
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);
            
        /// <summary>
        /// 異步獲取所有已過期的任務
        /// </summary>
        /// <returns>返回所有已過期但未完成的任務列表</returns>
        /// <remarks>
        /// 過期判定規則：
        /// 1. 任務未完成
        /// 2. 有設置截止日期
        /// 3. 當前時間已超過截止日期
        /// 
        /// 使用場景：
        /// - 發送過期任務提醒
        /// - 生成過期任務報告
        /// - 任務狀態監控
        /// </remarks>
        Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();

        /// <summary>
        /// 異步獲取指定優先級的所有任務
        /// </summary>
        /// <param name="priority">要查詢的優先級</param>
        /// <returns>返回指定優先級的任務列表</returns>
        /// <remarks>
        /// 查詢規則：
        /// - 返回所有指定優先級的任務，包括已完成和未完成的
        /// - 按創建時間降序排序
        /// 
        /// 使用場景：
        /// - 優先級任務管理
        /// - 工作量評估
        /// - 任務優先級分析
        /// </remarks>
        Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority);

        /// <summary>
        /// 異步添加新任務
        /// </summary>
        /// <param name="task">要添加的任務實體，不能為null</param>
        /// <returns>返回添加後的任務實體（包含生成的ID）</returns>
        /// <remarks>
        /// 添加規則：
        /// 1. 自動設置創建時間和更新時間
        /// 2. 生成唯一的任務ID
        /// 3. 驗證任務數據的完整性
        /// 
        /// 注意事項：
        /// - 需要驗證任務實體的必填字段
        /// - 應處理可能的並發問題
        /// - 確保數據的一致性
        /// </remarks>
        Task<TodoTask> AddAsync(TodoTask task);

        /// <summary>
        /// 異步更新現有任務
        /// </summary>
        /// <param name="task">要更新的任務實體，不能為null</param>
        /// <returns>更新操作的異步任務</returns>
        /// <remarks>
        /// 更新規則：
        /// 1. 自動更新最後修改時間
        /// 2. 只更新已變更的字段
        /// 3. 保留創建時間不變
        /// 
        /// 注意事項：
        /// - 需要驗證任務ID是否存在
        /// - 處理並發更新的情況
        /// - 確保數據的一致性
        /// </remarks>
        Task UpdateAsync(TodoTask task);

        /// <summary>
        /// 異步刪除指定ID的任務
        /// </summary>
        /// <param name="id">要刪除的任務ID，必須大於0</param>
        /// <returns>刪除操作的異步任務</returns>
        /// <remarks>
        /// 刪除規則：
        /// 1. 物理刪除，完全從數據庫中移除
        /// 2. 刪除前驗證任務是否存在
        /// 
        /// 注意事項：
        /// - 確保要刪除的任務存在
        /// - 處理關聯數據的刪除
        /// - 考慮是否需要軟刪除
        /// </remarks>
        Task DeleteAsync(int id);

        /// <summary>
        /// 異步檢查指定ID的任務是否存在
        /// </summary>
        /// <param name="id">要檢查的任務ID，必須大於0</param>
        /// <returns>如果任務存在則返回true，否則返回false</returns>
        /// <remarks>
        /// 使用場景：
        /// 1. 任務更新前的存在性檢查
        /// 2. 任務刪除前的驗證
        /// 3. API接口的參數驗證
        /// 
        /// 實現建議：
        /// - 使用輕量級查詢
        /// - 只檢查ID是否存在，不獲取任務詳情
        /// - 可以考慮使用緩存優化性能
        /// </remarks>
        Task<bool> ExistsAsync(int id);
    }
}
