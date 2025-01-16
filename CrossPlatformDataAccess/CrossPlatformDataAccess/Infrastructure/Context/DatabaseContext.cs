using CrossPlatformDataAccess.Common.Configuration;
using CrossPlatformDataAccess.Common.Enums;
using CrossPlatformDataAccess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CrossPlatformDataAccess.Infrastructure.Context
{
    /// <summary>
    /// 關聯式資料庫上下文實作，提供資料庫連線和操作的基礎功能
    /// </summary>
    public class DatabaseContext : DbContext, IDbContext
    {
        /// <summary>
        /// 資料庫設定物件
        /// </summary>
        // 儲存資料庫連線字串、資料庫類型等設定資訊
        private readonly DatabaseConfig _config;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="config">資料庫設定物件</param>
        // 透過依賴注入取得資料庫設定
        public DatabaseContext(DatabaseConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 設定資料庫連線和相關選項
        /// </summary>
        /// <param name="optionsBuilder">資料庫選項建構器</param>
        // 根據設定的資料庫類型建立對應的資料庫連線
        // 支援 SQL Server、PostgreSQL、MySQL 和 SQLite
        // 每種資料庫都啟用了重試機制以提高可靠性
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_config.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    // 設定 SQL Server 連線，並啟用連線失敗重試機制
                    optionsBuilder.UseSqlServer(
                        _config.ConnectionString,
                        options => options.EnableRetryOnFailure()
                    );
                    break;

                case DatabaseType.PostgreSQL:
                    // 設定 PostgreSQL 連線，並啟用連線失敗重試機制
                    optionsBuilder.UseNpgsql(
                        _config.ConnectionString,
                        options => options.EnableRetryOnFailure()
                    );
                    break;

                case DatabaseType.MySQL:
                    // 設定 MySQL 連線，自動偵測伺服器版本，並啟用連線失敗重試機制
                    optionsBuilder.UseMySql(
                        _config.ConnectionString,
                        ServerVersion.AutoDetect(_config.ConnectionString),
                        options => options.EnableRetryOnFailure()
                    );
                    break;

                case DatabaseType.SQLite:
                    // 設定 SQLite 連線
                    optionsBuilder.UseSqlite(_config.ConnectionString);
                    break;

                default:
                    throw new ArgumentException("不支援的資料庫類型", nameof(_config.DatabaseType));
            }

            // 啟用詳細的錯誤訊息（僅開發環境使用）
            // 可以看到更詳細的錯誤資訊，方便除錯
            optionsBuilder.EnableDetailedErrors();
            // 啟用敏感資料記錄，可以看到參數值
            optionsBuilder.EnableSensitiveDataLogging();
        }

        /// <summary>
        /// 取得資料庫連線物件
        /// </summary>
        /// <returns>資料庫連線物件</returns>
        // 提供底層資料庫連線物件的存取
        public DbConnection GetConnection() => Database.GetDbConnection();

        /// <summary>
        /// 非同步儲存所有變更至資料庫
        /// </summary>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>受影響的資料列數</returns>
        /// <exception cref="Exception">當儲存資料發生錯誤時拋出例外</exception>
        // 覆寫 SaveChangesAsync 方法，加入例外處理機制
        // 當發生資料庫更新錯誤時，包裝例外並提供更明確的錯誤訊息
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // 包裝資料庫更新例外，提供更清楚的錯誤訊息
                throw new Exception("儲存資料時發生錯誤", ex);
            }
        }
    }
}