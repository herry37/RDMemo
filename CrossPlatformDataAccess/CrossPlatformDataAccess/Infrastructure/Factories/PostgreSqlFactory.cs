using CrossPlatformDataAccess.Common.Configuration;
using CrossPlatformDataAccess.Core.Interfaces;
using Npgsql;
using System.Data;

namespace CrossPlatformDataAccess.Infrastructure.Factories
{
    /// <summary>
    /// PostgreSQL 資料庫工廠實作
    /// </summary>
    public class PostgreSqlFactory : IDbFactory
    {
        private readonly DatabaseConfig _config;

        /// <summary>
        /// 建構子，接受資料庫配置的注入
        /// </summary>
        /// <param name="config">資料庫配置</param>
        public PostgreSqlFactory(DatabaseConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 創建資料庫連線
        /// </summary>
        /// <returns>資料庫連線物件</returns>
        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_config.ConnectionString);
        }
    }
}
