using System;
using System.Data;
using System.Data.SqlClient;

namespace TableToModel
{
    /// <summary>
    /// SQL Server 資料庫連接工廠
    /// </summary>
    public class SqlDatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        /// <summary>
        /// SQL Server 資料庫連接
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <returns>資料庫連線</returns>
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// SQL Server 取得連線字串
        /// </summary>
        /// <param name="server">伺服器位址</param>
        /// <param name="userId">使用者名稱</param>
        /// <param name="password">密碼</param>
        /// <param name="database">資料庫名稱</param>
        /// <returns>連線字串</returns>
        public string GetConnectionString(string server, string userId, string password, string database)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                UserID = userId,
                Password = password,
                ConnectTimeout = 3,        // 增加到3秒
                Pooling = true,            // 啟用連線池
                MinPoolSize = 1,
                MaxPoolSize = 100,
                PersistSecurityInfo = false,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true,
                ApplicationIntent = ApplicationIntent.ReadWrite,
                Encrypt = false            // 本地連線不需要加密
            };

            return builder.ToString();
        }
    }
}
