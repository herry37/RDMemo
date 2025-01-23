// 引入必要的命名空間
using Microsoft.EntityFrameworkCore; // 用於數據庫操作
using TodoTaskManagementAPI.Domain; // 引入領域模型
using TodoTaskManagementAPI.Infrastructure; // 引入基礎設施層組件

namespace TodoTaskManagementAPI.Services
{
    /// <summary>
    /// 待辦事項任務服務實現類
    /// 提供任務的創建、查詢、更新和刪除等核心業務邏輯
    /// </summary>
    /// <remarks>
    /// 此服務類負責：
    /// 1. 處理任務的所有業務邏輯
    /// 2. 與數據庫進行交互
    /// 3. 提供任務的分頁查詢
    /// 4. 處理任務的優先級排序
    /// </remarks>
    public class TodoTaskService : ITodoTaskService
    {
        // 數據庫上下文實例，用於數據庫操作
        private readonly TodoTaskContext _context;

        /// <summary>
        /// 構造函數，注入數據庫上下文
        /// </summary>
        /// <param name="context">任務數據庫上下文</param>
        /// <remarks>
        /// 通過依賴注入獲取數據庫上下文實例
        /// 確保數據庫操作的一致性
        /// </remarks>
        public TodoTaskService(TodoTaskContext context)
        {
            _context = context; // 初始化數據庫上下文
        }

        /// <summary>
        /// 創建新的待辦事項任務
        /// </summary>
        /// <param name="task">要創建的任務實體</param>
        /// <returns>創建成功的任務</returns>
        /// <remarks>
        /// 1. 將新任務添加到數據庫
        /// 2. 保存更改
        /// 3. 返回包含自動生成ID的任務實體
        /// </remarks>
        public async Task<TodoTask> CreateTaskAsync(TodoTask task)
        {
            _context.TodoTasks.Add(task); // 將任務添加到數據庫上下文
            await _context.SaveChangesAsync(); // 保存更改到數據庫
            return task; // 返回已創建的任務
        }

        /// <summary>
        /// 刪除指定的待辦事項任務
        /// </summary>
        /// <param name="task">要刪除的任務實體</param>
        /// <remarks>
        /// 1. 從數據庫中移除指定任務
        /// 2. 保存更改
        /// </remarks>
        public async Task DeleteTaskAsync(TodoTask task)
        {
            _context.TodoTasks.Remove(task); // 從數據庫中移除任務
            await _context.SaveChangesAsync(); // 保存更改到數據庫
        }

        /// <summary>
        /// 根據ID獲取待辦事項任務
        /// </summary>
        /// <param name="id">任務ID</param>
        /// <returns>找到的任務實體，如果不存在則返回null</returns>
        /// <remarks>
        /// 使用 EF Core 的 FindAsync 方法高效查詢單個任務
        /// </remarks>
        public async Task<TodoTask?> GetTaskByIdAsync(int id)
        {
            return await _context.TodoTasks.FindAsync(id); // 根據ID查找任務
        }

        /// <summary>
        /// 獲取分頁的待辦事項任務列表
        /// </summary>
        /// <param name="page">頁碼，從1開始</param>
        /// <param name="pageSize">每頁顯示的記錄數</param>
        /// <param name="isCompleted">是否已完成的過濾條件</param>
        /// <param name="priority">優先級過濾條件</param>
        /// <returns>分頁結果，包含任務列表和分頁信息</returns>
        /// <remarks>
        /// 實現功能：
        /// 1. 分頁查詢
        /// 2. 條件過濾
        /// 3. 優先級排序
        /// 4. 創建時間排序
        /// </remarks>
        public async Task<PagedResult<TodoTask>> GetAllAsync(int page = 1, int pageSize = 10, bool? isCompleted = null, Priority? priority = null)
        {
            // 確保頁碼和頁面大小有效
            page = Math.Max(1, page); // 確保頁碼最小為1
            pageSize = Math.Clamp(pageSize, 1, 50); // 限制每頁記錄數在1到50之間

            // 構建基礎查詢
            IQueryable<TodoTask> query = _context.TodoTasks;

            // 應用過濾條件
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value); // 根據完成狀態過濾
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value); // 根據優先級過濾
            }

            // 計算分頁信息
            var totalItems = await query.CountAsync(); // 獲取總記錄數
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize); // 計算總頁數

            // 獲取當前頁的數據
            var items = await query
                .OrderByDescending(t => t.Priority) // 優先按優先級降序排序
                .ThenByDescending(t => t.CreatedAt) // 然後按創建時間降序排序
                .Skip((page - 1) * pageSize) // 跳過前面頁的記錄
                .Take(pageSize) // 獲取當前頁的記錄
                .ToListAsync();

            // 構造並返回分頁結果
            return new PagedResult<TodoTask>
            {
                Items = items, // 當前頁的任務列表
                TotalItems = totalItems, // 總記錄數
                CurrentPage = page, // 當前頁碼
                PageSize = pageSize, // 每頁記錄數
                TotalPages = totalPages // 總頁數
            };
        }

        /// <summary>
        /// 更新待辦事項任務
        /// </summary>
        /// <param name="task">要更新的任務實體</param>
        /// <returns>更新後的任務實體</returns>
        /// <remarks>
        /// 1. 更新任務的所有屬性
        /// 2. 保存更改到數據庫
        /// </remarks>
        public async Task<TodoTask> UpdateTaskAsync(TodoTask task)
        {
            _context.TodoTasks.Update(task); // 更新任務實體
            await _context.SaveChangesAsync(); // 保存更改到數據庫
            return task; // 返回更新後的任務
        }

        /// <summary>
        /// 獲取所有逾期的待辦事項任務
        /// </summary>
        /// <returns>逾期任務列表</returns>
        /// <remarks>
        /// 查詢條件：
        /// 1. 未完成的任務
        /// 2. 有截止日期的任務
        /// 3. 截止日期早於當前時間
        /// 按截止日期升序排序
        /// </remarks>
        public async Task<IEnumerable<TodoTask>> GetOverdueTasksAsync()
        {
            var now = DateTime.UtcNow; // 獲取當前UTC時間
            return await _context.TodoTasks
                .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < now) // 篩選逾期任務
                .OrderBy(t => t.DueDate) // 按截止日期升序排序
                .ToListAsync();
        }

        /// <summary>
        /// 根據優先級獲取待辦事項任務
        /// </summary>
        /// <param name="priority">要查詢的優先級</param>
        /// <returns>指定優先級的任務列表</returns>
        /// <remarks>
        /// 1. 篩選指定優先級的任務
        /// 2. 按創建時間降序排序
        /// </remarks>
        public async Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority)
        {
            return await _context.TodoTasks
                .Where(t => t.Priority == priority) // 根據優先級過濾
                .OrderByDescending(t => t.CreatedAt) // 按創建時間降序排序
                .ToListAsync();
        }
    }
}
