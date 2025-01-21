namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 分頁響應模型
    /// </summary>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// 當前頁的數據項
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// 總記錄數
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// 當前頁碼
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 每頁記錄數
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// 默認構造函數
        /// </summary>
        public PaginatedResponse()
        {
            Items = new List<T>();
        }

        /// <summary>
        /// 構造函數
        /// </summary>
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
