using System.Data.Common;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 資料庫提供者介面
    /// </summary>
    public interface IDbProvider
    {
        /// <summary>
        /// 取得資料庫連線字串
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 取得資料庫類型
        /// </summary>
        DatabaseType DatabaseType { get; }

        #region 連線管理

        /// <summary>
        /// 建立資料庫連線
        /// </summary>
        DbConnection CreateConnection();

        /// <summary>
        /// 開啟連線
        /// </summary>
        void OpenConnection(DbConnection connection);

        /// <summary>
        /// 非同步開啟連線
        /// </summary>
        Task OpenConnectionAsync(DbConnection connection, CancellationToken cancellationToken = default);

        #endregion

        #region 資料表操作

        /// <summary>
        /// 取得資料庫所有資料表
        /// </summary>
        IEnumerable<string> GetAllTables();

        /// <summary>
        /// 非同步取得資料庫所有資料表
        /// </summary>
        Task<IEnumerable<string>> GetAllTablesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得資料表結構
        /// </summary>
        IEnumerable<TableSchema> GetTableSchema(string tableName);

        /// <summary>
        /// 非同步取得資料表結構
        /// </summary>
        Task<IEnumerable<TableSchema>> GetTableSchemaAsync(string tableName, CancellationToken cancellationToken = default);

        #endregion

        #region SQL 指令產生

        /// <summary>
        /// 產生分頁查詢的 SQL
        /// </summary>
        string GeneratePagedQuery(string sql, int pageNumber, int pageSize);

        /// <summary>
        /// 產生新增資料的 SQL
        /// </summary>
        string GenerateInsertQuery(string tableName, IEnumerable<string> columns);

        /// <summary>
        /// 產生更新資料的 SQL
        /// </summary>
        string GenerateUpdateQuery(string tableName, IEnumerable<string> columns, IEnumerable<string> whereColumns);

        /// <summary>
        /// 產生刪除資料的 SQL
        /// </summary>
        string GenerateDeleteQuery(string tableName, IEnumerable<string> whereColumns);

        #endregion
    }

    /// <summary>
    /// 資料庫類型
    /// </summary>
    public enum DatabaseType
    {
        SqlServer,
        MySql,
        PostgreSql,
        Sqlite
    }

    /// <summary>
    /// 資料表結構
    /// </summary>
    public class TableSchema
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public string DefaultValue { get; set; }
    }
}
