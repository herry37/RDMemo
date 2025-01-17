using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 定義資料存取策略的介面
    /// 可以是 EF Core, Dapper, ADO.NET 等不同實現
    /// </summary>
    public interface IDataAccessStrategy
    {
        #region 連線管理

        /// <summary>
        /// 取得資料庫連線
        /// </summary>
        DbConnection GetConnection();

        /// <summary>
        /// 開啟連線
        /// </summary>
        void OpenConnection();

        /// <summary>
        /// 非同步開啟連線
        /// </summary>
        Task OpenConnectionAsync(CancellationToken cancellationToken = default);

        #endregion

        #region 交易管理

        /// <summary>
        /// 開始交易
        /// </summary>
        ITransactionScope BeginTransaction();

        /// <summary>
        /// 非同步開始交易
        /// </summary>
        Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default);

        #endregion

        #region 查詢操作

        /// <summary>
        /// 取得查詢建構器
        /// </summary>
        IQueryBuilder<T> Query<T>() where T : class;

        /// <summary>
        /// 執行查詢並返回結果集
        /// </summary>
        IEnumerable<T> Query<T>(string sql, object parameters = null) where T : class;

        /// <summary>
        /// 非同步執行查詢並返回結果集
        /// </summary>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 執行命令並返回影響的行數
        /// </summary>
        int Execute(string sql, object parameters = null);

        /// <summary>
        /// 非同步執行命令並返回影響的行數
        /// </summary>
        Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default);

        #endregion
    }

    /// <summary>
    /// 交易範圍介面
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// 提交交易
        /// </summary>
        void Commit();

        /// <summary>
        /// 非同步提交交易
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 回滾交易
        /// </summary>
        void Rollback();

        /// <summary>
        /// 非同步回滾交易
        /// </summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
