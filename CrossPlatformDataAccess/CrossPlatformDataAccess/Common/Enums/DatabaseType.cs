namespace CrossPlatformDataAccess.Common.Enums
{
    /// <summary>
    /// 資料庫類型列舉
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// Microsoft SQL Server 資料庫
        /// </summary>
        SqlServer,

        /// <summary>
        /// PostgreSQL 開源關聯式資料庫
        /// </summary>
        PostgreSQL,

        /// <summary>
        /// MySQL 開源關聯式資料庫
        /// </summary>
        MySQL,

        /// <summary>
        /// SQLite 輕量級關聯式資料庫
        /// </summary>
        SQLite,

        /// <summary>
        /// MongoDB NoSQL 文件導向資料庫
        /// </summary>
        MongoDB
    }
}