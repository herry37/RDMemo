using CrossPlatformDataAccess.Common.Enums;

namespace CrossPlatformDataAccess.Common.Configuration
{
    /// <summary>
    /// 關聯式資料庫設定
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// 資料庫連線字串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 資料庫類型
        /// </summary>
        public DatabaseType DatabaseType { get; set; }
    }
}