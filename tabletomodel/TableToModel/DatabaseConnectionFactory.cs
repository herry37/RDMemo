namespace TableToModel
{
    /// <summary>
    /// 資料庫連接工廠類，用於創建具體資料庫連接工廠的實例
    /// </summary>
    public static class DatabaseConnectionFactory
    {
        /// <summary>
        /// 取得資料庫連線工廠實例
        /// </summary>
        /// <param name="dbType">資料庫類型</param>
        /// <returns>資料庫連線工廠實例</returns>
        /// <exception cref="ArgumentException">當指定的資料庫類型無效時拋出</exception>
        public static IDatabaseConnectionFactory GetFactory(DatabaseType dbType)
        {
            return dbType switch
            {
                DatabaseType.MSSQL => new SqlDatabaseConnectionFactory(),
                DatabaseType.MySQL => new MySqlDatabaseConnectionFactory(),
                DatabaseType.MongoDB => new MongoDbConnectionFactory(),
                _ => throw new ArgumentException($"不支援的資料庫類型: {dbType}", nameof(dbType))
            };
        }

        /// <summary>
        /// 建立資料庫連線
        /// </summary>
        /// <param name="dbType">資料庫類型</param>
        /// <param name="server">伺服器位址</param>
        /// <param name="userId">使用者名稱</param>
        /// <param name="password">密碼</param>
        /// <param name="database">資料庫名稱</param>
        /// <returns>資料庫連線</returns>
        /// <exception cref="ArgumentNullException">當任何必要參數為 null 或空白時拋出</exception>
        /// <exception cref="DatabaseConnectionException">當建立連線時發生錯誤時拋出</exception>
        public static IDbConnection CreateConnection(DatabaseType dbType, string server, string userId, string password, string database)
        {
            ValidateConnectionParameters(server, userId, password, database);

            try
            {
                var factory = GetFactory(dbType);
                var connectionString = factory.GetConnectionString(server, userId, password, database);
                var connection = factory.CreateConnection(connectionString);

                return connection;
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not ArgumentException)
            {
                throw new DatabaseConnectionException("建立資料庫連線時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 驗證連線參數
        /// </summary>
        private static void ValidateConnectionParameters(string server, string userId, string password, string database)
        {
            if (string.IsNullOrWhiteSpace(server))
                throw new ArgumentNullException(nameof(server), "伺服器位址不能為空");

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId), "使用者名稱不能為空");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password), "密碼不能為空");

            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentNullException(nameof(database), "資料庫名稱不能為空");
        }

        /// <summary>
        /// 資料庫類型
        /// </summary>
        public enum DatabaseType
        {
            /// <summary>
            /// SQL Server
            /// </summary>
            MSSQL,

            /// <summary>
            /// MySQL
            /// </summary>
            MySQL,

            /// <summary>
            /// MongoDB
            /// </summary>
            MongoDB
        }
    }
}
