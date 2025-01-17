using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 資料存取策略介面
    /// </summary>
    public interface IDataAccessStrategy
    {
        /// <summary>
        /// 取得資料庫提供者
        /// </summary>
        IDbProvider Provider { get; }

        #region SQL 操作

        /// <summary>
        /// 執行 SQL 查詢並返回實體列表 (同步)
        /// </summary>
        IEnumerable<T> Query<T>(string sql, object? parameters = null) where T : class;

        /// <summary>
        /// 執行 SQL 查詢並返回實體列表 (非同步)
        /// </summary>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 執行 SQL 命令 (同步)
        /// </summary>
        int Execute(string sql, object? parameters = null);

        /// <summary>
        /// 執行 SQL 命令 (非同步)
        /// </summary>
        Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 執行預存程序 (同步)
        /// </summary>
        IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, object? parameters = null) where T : class;

        /// <summary>
        /// 執行預存程序 (非同步)
        /// </summary>
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null, CancellationToken cancellationToken = default) where T : class;

        #endregion

        #region CRUD 操作

        /// <summary>
        /// 新增實體 (同步)
        /// </summary>
        T Add<T>(T entity) where T : class;

        /// <summary>
        /// 新增實體 (非同步)
        /// </summary>
        Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 更新實體 (同步)
        /// </summary>
        bool Update<T>(T entity) where T : class;

        /// <summary>
        /// 更新實體 (非同步)
        /// </summary>
        Task<bool> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 刪除實體 (同步)
        /// </summary>
        bool Delete<T>(T entity) where T : class;

        /// <summary>
        /// 刪除實體 (非同步)
        /// </summary>
        Task<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 根據 ID 取得實體 (同步)
        /// </summary>
        T GetById<T>(object id) where T : class;

        /// <summary>
        /// 根據 ID 取得實體 (非同步)
        /// </summary>
        Task<T> GetByIdAsync<T>(object id, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 取得所有實體 (同步)
        /// </summary>
        IEnumerable<T> GetAll<T>() where T : class;

        /// <summary>
        /// 取得所有實體 (非同步)
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class;

        #endregion

        #region 查詢建構

        /// <summary>
        /// 建立查詢建構器
        /// </summary>
        IQueryBuilder<T> Query<T>() where T : class;

        #endregion

        #region 交易管理

        /// <summary>
        /// 開始交易 (同步)
        /// </summary>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// 開始交易 (非同步)
        /// </summary>
        Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 在交易中執行操作 (同步)
        /// </summary>
        T ExecuteInTransaction<T>(Func<T> operation);

        /// <summary>
        /// 在交易中執行操作 (非同步)
        /// </summary>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);

        #endregion

        #region 批次操作

        /// <summary>
        /// 批次新增實體 (同步)
        /// </summary>
        void BulkInsert<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 批次新增實體 (非同步)
        /// </summary>
        Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 批次更新實體 (同步)
        /// </summary>
        void BulkUpdate<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 批次更新實體 (非同步)
        /// </summary>
        Task BulkUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 批次刪除實體 (同步)
        /// </summary>
        void BulkDelete<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 批次刪除實體 (非同步)
        /// </summary>
        Task BulkDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class;

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
