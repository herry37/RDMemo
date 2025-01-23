/// <summary>
/// 任務管理 API 的分頁響應模型定義
/// 用於處理大量數據的分頁顯示
/// </summary>

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 通用分頁響應模型
    /// 支持任意類型數據的分頁展示
    /// </summary>
    /// <typeparam name="T">分頁數據的類型參數</typeparam>
    /// <remarks>
    /// 此模型用於：
    /// 1. 實現數據的分頁展示
    /// 2. 提供分頁相關的元數據
    /// 3. 支持分頁導航功能
    /// 
    /// 特點：
    /// - 泛型設計，支持任意數據類型
    /// - 包含完整的分頁信息
    /// - 提供分頁導航輔助屬性
    /// 
    /// 使用場景：
    /// - 列表數據展示
    /// - 搜索結果分頁
    /// - 大數據集瀏覽
    /// </remarks>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// 當前頁的數據集合
        /// 包含當前頁的所有數據項
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 使用 IEnumerable 接口提供數據訪問
        /// 2. 支持延遲加載
        /// 3. 允許為空集合
        /// 
        /// 注意事項：
        /// - 數據項數量不超過 PageSize
        /// - 最後一頁可能不滿 PageSize
        /// - 空集合表示無數據
        /// </remarks>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// 數據集的總記錄數
        /// 表示未分頁時的總數據量
        /// </summary>
        /// <remarks>
        /// 用途：
        /// 1. 計算總頁數
        /// 2. 顯示數據統計
        /// 3. 驗證分頁參數
        /// 
        /// 特點：
        /// - 包含過濾後的總數
        /// - 獨立於當前頁的數據量
        /// - 用於分頁計算
        /// </remarks>
        public int TotalItems { get; set; }

        /// <summary>
        /// 當前頁碼
        /// 表示用戶正在查看的頁面編號
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 從 1 開始計數
        /// 2. 不能大於總頁數
        /// 3. 用於分頁導航
        /// 
        /// 驗證規則：
        /// - 必須大於 0
        /// - 不應超過總頁數
        /// - 用於計算數據偏移量
        /// </remarks>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 每頁記錄數
        /// 定義每頁最多顯示的數據項數量
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 影響數據查詢效率
        /// 2. 影響頁面展示效果
        /// 3. 用於分頁計算
        /// 
        /// 建議：
        /// - 根據數據大小設置合適的值
        /// - 考慮用戶體驗和性能平衡
        /// - 可以作為用戶可配置項
        /// </remarks>
        public int PageSize { get; set; }

        /// <summary>
        /// 總頁數
        /// 根據總記錄數和每頁記錄數計算得出
        /// </summary>
        /// <remarks>
        /// 計算方式：
        /// - 總頁數 = 向上取整(總記錄數 / 每頁記錄數)
        /// 
        /// 特點：
        /// 1. 自動計算，不需手動設置
        /// 2. 最小值為 1（當有數據時）
        /// 3. 用於分頁導航限制
        /// </remarks>
        public int TotalPages { get; set; }

        /// <summary>
        /// 是否存在上一頁
        /// 用於確定是否可以導航到前一頁
        /// </summary>
        /// <remarks>
        /// 計算規則：
        /// - 當前頁碼大於 1 時返回 true
        /// - 第一頁時返回 false
        /// 
        /// 用途：
        /// - 控制上一頁按鈕狀態
        /// - 分頁導航邏輯判斷
        /// - UI 交互優化
        /// </remarks>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// 是否存在下一頁
        /// 用於確定是否可以導航到後一頁
        /// </summary>
        /// <remarks>
        /// 計算規則：
        /// - 當前頁碼小於總頁數時返回 true
        /// - 最後一頁時返回 false
        /// 
        /// 用途：
        /// - 控制下一頁按鈕狀態
        /// - 分頁導航邏輯判斷
        /// - UI 交互優化
        /// </remarks>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// 默認構造函數
        /// 創建一個空的分頁響應對象
        /// </summary>
        /// <remarks>
        /// 用途：
        /// 1. 創建空的分頁結果
        /// 2. 初始化基本屬性
        /// 3. 避免空引用異常
        /// 
        /// 初始化：
        /// - Items 初始化為空集合
        /// - 其他數值型屬性默認為 0
        /// </remarks>
        public PaginatedResponse()
        {
            Items = new List<T>();
        }

        /// <summary>
        /// 帶參數的構造函數
        /// 創建包含完整分頁信息的響應對象
        /// </summary>
        /// <param name="items">當前頁的數據集合</param>
        /// <param name="totalItems">總記錄數</param>
        /// <param name="currentPage">當前頁碼</param>
        /// <param name="pageSize">每頁記錄數</param>
        /// <remarks>
        /// 功能：
        /// 1. 初始化所有分頁屬性
        /// 2. 自動計算總頁數
        /// 3. 處理空集合情況
        /// 
        /// 參數驗證：
        /// - items 為 null 時使用空集合
        /// - totalItems 必須大於等於 0
        /// - currentPage 必須大於 0
        /// - pageSize 必須大於 0
        /// 
        /// 計算邏輯：
        /// - 總頁數 = 向上取整(totalItems / pageSize)
        /// - 使用 Math.Ceiling 確保有餘數時多一頁
        /// </remarks>
        public PaginatedResponse(IEnumerable<T> items, int totalItems, int currentPage, int pageSize)
        {
            Items = items ?? new List<T>();
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }
    }
}
