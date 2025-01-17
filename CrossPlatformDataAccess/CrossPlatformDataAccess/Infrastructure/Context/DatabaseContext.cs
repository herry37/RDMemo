using CrossPlatformDataAccess.Common.Configuration;
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
        private readonly DatabaseConfig _config;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="config">資料庫設定物件</param>
        public DatabaseContext(DatabaseConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            if (!_config.Validate())
            {
                throw new ArgumentException("資料庫設定無效", nameof(config));
            }
        }

        /// <summary>
        /// 設定資料庫連線和相關選項
        /// </summary>
        /// <param name="optionsBuilder">資料庫選項建構器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            // 根據設定的資料庫類型建立對應的資料庫連線
            // 支援 SQL Server、PostgreSQL、MySQL 和 SQLite
            // 每種資料庫都啟用了重試機制以提高可靠性
            switch (_config.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    optionsBuilder.UseSqlServer(_config.GetConnectionString(),
                        options => options.EnableRetryOnFailure());
                    break;
                case DatabaseType.PostgreSql:
                    optionsBuilder.UseNpgsql(_config.GetConnectionString(),
                        options => options.EnableRetryOnFailure());
                    break;
                case DatabaseType.MySql:
                    optionsBuilder.UseMySql(_config.GetConnectionString(),
                        ServerVersion.AutoDetect(_config.GetConnectionString()),
                        options => options.EnableRetryOnFailure());
                    break;
                case DatabaseType.Sqlite:
                    optionsBuilder.UseSqlite(_config.GetConnectionString());
                    break;
                default:
                    throw new NotSupportedException($"不支援的資料庫類型: {_config.DatabaseType}");
            }
        }

        /// <summary>
        /// 取得資料庫連線物件
        /// </summary>
        /// <returns>資料庫連線物件</returns>
        public DbConnection GetConnection()
        {
            var connection = Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        /// <summary>
        /// 非同步儲存所有變更至資料庫
        /// </summary>
        /// <param name="cancellationToken">取消權杖</param>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("資料已被其他使用者修改，請重新載入後再試", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("儲存資料時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 取得指定實體類型的 DbSet
        /// </summary>
        /// <typeparam name="TEntity">實體類型</typeparam>
        /// <returns>對應的 DbSet</returns>
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}