namespace BackendManagement.Domain.Constants;

/// <summary>
/// 系統常數定義
/// </summary>
public static class SystemConstants
{
    /// <summary>
    /// 資料庫相關常數
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// 預設的資料批次處理大小
        /// </summary>
        public const int DefaultBatchSize = 100;

        /// <summary>
        /// 最大並行處理數
        /// </summary>
        public const int MaxConcurrency = 5;

        /// <summary>
        /// 資料庫欄位長度限制
        /// </summary>
        public static class FieldLength
        {
            public const int Username = 50;
            public const int Email = 100;
            public const int Password = 100;
        }
    }

    /// <summary>
    /// 日誌相關常數
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// 預設日誌保存天數
        /// </summary>
        public const int DefaultRetentionDays = 30;

        /// <summary>
        /// 預設日誌目錄名稱
        /// </summary>
        public const string DefaultLogDirectory = "Logs";
    }
} 