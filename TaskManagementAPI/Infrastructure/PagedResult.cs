using System.Collections.Generic; // 引入泛型集合命名空間

namespace TodoTaskManagementAPI.Infrastructure
{
    /// <summary>
    /// 分頁結果泛型類別
    /// 用於封裝分頁查詢的結果數據
    /// </summary>
    /// <typeparam name="T">分頁數據的類型</typeparam>
    /// <remarks>
    /// 此類別用於：
    /// 1. 提供統一的分頁數據結構
    /// 2. 支持任意類型的數據分頁
    /// 3. 包含分頁的元數據（總數、頁碼等）
    /// 4. 便於前端實現分頁顯示
    /// </remarks>
    public class PagedResult<T>
    {
        /// <summary>
        /// 當前頁的數據項集合
        /// </summary>
        /// <remarks>
        /// - 包含當前頁的所有數據項
        /// - 數據類型由泛型參數 T 決定
        /// - 默認為空列表
        /// </remarks>
        public IEnumerable<T> Items { get; set; } = new List<T>(); // 當前頁的數據集合，初始化為空列表

        /// <summary>
        /// 數據總條數
        /// </summary>
        /// <remarks>
        /// - 所有符合條件的數據總數
        /// - 用於計算總頁數
        /// - 用於顯示數據總量
        /// </remarks>
        public int TotalItems { get; set; } // 數據總條數

        /// <summary>
        /// 當前頁碼
        /// </summary>
        /// <remarks>
        /// - 從 1 開始的頁碼
        /// - 用於分頁導航
        /// - 用於數據查詢時的偏移量計算
        /// </remarks>
        public int CurrentPage { get; set; } // 當前頁碼

        /// <summary>
        /// 每頁顯示的數據條數
        /// </summary>
        /// <remarks>
        /// - 決定每頁顯示多少條數據
        /// - 用於分頁大小的控制
        /// - 用於數據查詢時的限制
        /// </remarks>
        public int PageSize { get; set; } // 每頁數據條數

        /// <summary>
        /// 總頁數
        /// </summary>
        /// <remarks>
        /// - 根據總條數和每頁條數計算
        /// - 用於分頁導航
        /// - 用於判斷是否為最後一頁
        /// </remarks>
        public int TotalPages { get; set; } // 總頁數
    }
}
