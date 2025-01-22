using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace TableToModel
{
    /// <summary>
    /// MySQL 資料庫連接工廠
    /// </summary>
    public class MySqlDatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        /// <summary>
        /// 創建 MySQL 資料庫連接
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <returns>資料庫連線</returns>
        public IDbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// 生成 MySQL 資料庫連接字串
        /// </summary>
        /// <param name="server">伺服器位址</param>
        /// <param name="userId">使用者名稱</param>
        /// <param name="password">密碼</param>
        /// <param name="database">資料庫名稱</param>
        /// <returns>連線字串</returns>
        public string GetConnectionString(string server, string userId, string password, string database)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = database,
                UserID = userId,
                Password = password,
                ConnectionTimeout = 1,    // 設定1秒超時
                Pooling = true,           // 啟用連線池
                MinimumPoolSize = 1,
                MaximumPoolSize = 100
            };

            return builder.ToString();
        }
    }
}
