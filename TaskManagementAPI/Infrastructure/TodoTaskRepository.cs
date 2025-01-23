using Microsoft.EntityFrameworkCore;
using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Infrastructure
{
    /// <summary>
    /// TodoTask 倉儲實現類
    /// 實現 ITodoTaskRepository 介面，負責任務數據的持久化操作
    /// </summary>
    /// <remarks>
    /// 此類別提供：
    /// 1. 基本的 CRUD 操作實現
    /// 2. 高級查詢功能（分頁、篩選）
    /// 3. 特定業務邏輯查詢（過期任務、優先級任務）
    /// 4. 數據一致性和事務管理
    /// 
    /// 實現特點：
    /// - 使用 Entity Framework Core 進行數據訪問
    /// - 支持異步操作
    /// - 實現查詢條件組合
    /// - 處理實體狀態管理
    /// </remarks>
    public class TodoTaskRepository : ITodoTaskRepository
    {
        /// <summary>
        /// 數據庫上下文實例
        /// </summary>
        /// <remarks>
        /// - 通過依賴注入獲得
        /// - 負責管理數據庫連接
        /// - 處理實體追蹤
        /// </remarks>
        private readonly TodoTaskContext _context;

        /// <summary>
        /// 構造函數
        /// </summary>
        /// <param name="context">數據庫上下文</param>
        /// <remarks>
        /// - 通過依賴注入容器創建實例
        /// - 初始化數據庫上下文
        /// </remarks>
        public TodoTaskRepository(TodoTaskContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根據ID異步獲取單個任務
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <returns>返回指定ID的任務，如果不存在則返回null</returns>
        /// <remarks>
        /// 查詢說明：
        /// - 使用 FindAsync 方法進行高效查詢
        /// - 支持實體緩存
        /// - 返回追蹤的實體實例
        /// </remarks>
        public async Task<TodoTask> GetByIdAsync(int id)
        {
            return await _context.TodoTasks.FindAsync(id);
        }

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
        /// <remarks>
        /// 查詢特點：
        /// 1. 支持多條件組合查詢
        /// 2. 實現分頁功能
        /// 3. 結果排序規則：
        ///    - 優先按優先級降序
        ///    - 其次按創建時間降序
        /// 4. 採用延遲執行策略
        /// </remarks>
        public async Task<IEnumerable<TodoTask>> GetAllAsync(
            int? skip = null,
            int? take = null,
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null)
        {
            var query = _context.TodoTasks.AsQueryable();

            // 根據完成狀態篩選
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            // 根據優先級篩選
            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            // 根據起始日期篩選
            if (dueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= dueDateFrom.Value);
            }

            // 根據結束日期篩選
            if (dueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= dueDateTo.Value);
            }

            // 設置排序規則
            query = query.OrderByDescending(t => t.Priority)
                        .ThenByDescending(t => t.CreatedAt);

            // 應用分頁
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

        /// <summary>
        /// 異步獲取符合條件的任務總數
        /// </summary>
        /// <param name="isCompleted">完成狀態過濾</param>
        /// <param name="priority">優先級過濾</param>
        /// <param name="dueDateFrom">截止日期起始</param>
        /// <param name="dueDateTo">截止日期結束</param>
        /// <returns>符合條件的記錄總數</returns>
        /// <remarks>
        /// 查詢特點：
        /// 1. 使用相同的過濾條件
        /// 2. 不考慮分頁參數
        /// 3. 採用高效的計數查詢
        /// 4. 用於分頁計算和統計
        /// </remarks>
        public async Task<int> GetTotalCountAsync(
            bool? isCompleted = null,
            Priority? priority = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null)
        {
            var query = _context.TodoTasks.AsQueryable();

            // 根據完成狀態篩選
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            // 根據優先級篩選
            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            // 根據起始日期篩選
            if (dueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= dueDateFrom.Value);
            }

            // 根據結束日期篩選
            if (dueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= dueDateTo.Value);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// 異步獲取所有已過期的任務
        /// </summary>
        /// <returns>過期且未完成的任務列表</returns>
        /// <remarks>
        /// 查詢條件：
        /// 1. 任務未完成
        /// 2. 有設置截止日期
        /// 3. 當前時間超過截止日期
        /// 
        /// 排序規則：
        /// 1. 優先級降序
        /// 2. 截止日期升序（先過期的排前面）
        /// </remarks>
        public async Task<IEnumerable<TodoTask>> GetOverdueTasksAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.TodoTasks
                .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        /// <summary>
        /// 異步獲取指定優先級的未完成任務
        /// </summary>
        /// <param name="priority">要查詢的優先級</param>
        /// <returns>指定優先級的未完成任務列表</returns>
        /// <remarks>
        /// 查詢條件：
        /// 1. 匹配指定優先級
        /// 2. 僅返回未完成的任務
        /// 
        /// 排序規則：
        /// - 按創建時間降序（新建立的排前面）
        /// </remarks>
        public async Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority)
        {
            return await _context.TodoTasks
                .Where(t => t.Priority == priority && !t.IsCompleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 異步添加新任務
        /// </summary>
        /// <param name="task">要添加的任務實體</param>
        /// <returns>添加後的任務實體（包含生成的ID）</returns>
        /// <remarks>
        /// 處理流程：
        /// 1. 驗證任務實體不為null
        /// 2. 添加到上下文
        /// 3. 保存更改
        /// 4. 返回包含新ID的實體
        /// 
        /// 異常處理：
        /// - 當task為null時拋出ArgumentNullException
        /// - 數據庫異常會拋出DbUpdateException
        /// </remarks>
        public async Task<TodoTask> AddAsync(TodoTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _context.TodoTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// 異步更新任務
        /// </summary>
        /// <param name="task">要更新的任務實體</param>
        /// <returns>更新後的任務實體</returns>
        /// <remarks>
        /// 處理流程：
        /// 1. 驗證任務實體不為null
        /// 2. 檢查實體追蹤狀態
        /// 3. 設置實體狀態為已修改
        /// 4. 保存更改
        /// 
        /// 狀態管理：
        /// - 處理分離狀態的實體
        /// - 自動追蹤更改
        /// - 優化更新性能
        /// </remarks>
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

        /// <summary>
        /// 異步刪除指定ID的任務
        /// </summary>
        /// <param name="id">要刪除的任務ID</param>
        /// <remarks>
        /// 處理流程：
        /// 1. 查找指定ID的任務
        /// 2. 如果存在則刪除
        /// 3. 保存更改
        /// 
        /// 特點：
        /// - 靜默處理不存在的ID
        /// - 採用物理刪除
        /// - 自動處理實體狀態
        /// </remarks>
        public async Task DeleteAsync(int id)
        {
            var task = await GetByIdAsync(id);
            if (task != null)
            {
                _context.TodoTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 異步檢查指定ID的任務是否存在
        /// </summary>
        /// <param name="id">要檢查的任務ID</param>
        /// <returns>如果存在返回true，否則返回false</returns>
        /// <remarks>
        /// 查詢優化：
        /// 1. 使用 AsNoTracking 提高性能
        /// 2. 使用 Any 而不是 Count
        /// 3. 只檢查ID存在性
        /// 
        /// 使用場景：
        /// - 任務更新前的檢查
        /// - API參數驗證
        /// - 避免重複操作
        /// </remarks>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TodoTasks
                .AsNoTracking()
                .AnyAsync(t => t.Id == id);
        }
    }
}
