using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CrossPlatformDataAccess.Core.DataAccess;
using CrossPlatformDataAccess.Core.Logging;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess
{
    /// <summary>
    /// 資料存取基礎類別
    /// 提供所有資料存取操作的基本實作
    /// </summary>
    public abstract class DataAccessBase<T> : IDataAccess<T> where T : class
    {
        protected readonly IDataAccessStrategy _strategy;
        protected readonly ILogger _logger;

        protected DataAccessBase(IDataAccessStrategy strategy, ILogger logger)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region CRUD 實作

        public virtual T Add(T entity)
        {
            return ExecuteInTransaction(() =>
            {
                _logger.LogInformation($"新增 {typeof(T).Name}");
                return ExecuteAdd(entity);
            });
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            return await ExecuteInTransactionAsync(async () =>
            {
                _logger.LogInformation($"新增 {typeof(T).Name}");
                return await ExecuteAddAsync(entity, cancellationToken);
            }, cancellationToken);
        }

        // 其他 CRUD 方法實作...

        #endregion

        #region SQL 操作實作

        public virtual IEnumerable<T> QueryWithSql(string sql, object parameters = null)
        {
            try
            {
                _logger.LogInformation($"執行SQL查詢: {sql}");
                return _strategy.Query<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL查詢失敗: {sql}", ex);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> QueryWithSqlAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"執行SQL查詢: {sql}");
                return await _strategy.QueryAsync<T>(sql, parameters, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL查詢失敗: {sql}", ex);
                throw;
            }
        }

        // 其他 SQL 操作方法實作...

        #endregion

        #region 交易管理實作

        public virtual TResult ExecuteInTransaction<TResult>(Func<TResult> operation)
        {
            using var transaction = _strategy.BeginTransaction();
            try
            {
                _logger.LogInformation("開始執行交易");
                var result = operation();
                transaction.Commit();
                _logger.LogInformation("交易執行成功");
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("交易執行失敗", ex);
                throw;
            }
        }

        public virtual async Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<Task<TResult>> operation,
            CancellationToken cancellationToken = default)
        {
            using var transaction = await _strategy.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogInformation("開始執行交易");
                var result = await operation();
                await transaction.CommitAsync();
                _logger.LogInformation("交易執行成功");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("交易執行失敗", ex);
                throw;
            }
        }

        // 其他交易方法實作...

        #endregion

        #region 查詢建構器

        public virtual IQueryBuilder<T> Query()
        {
            return _strategy.Query<T>();
        }

        #endregion

        #region 需要被實作的抽象方法

        protected abstract T ExecuteAdd(T entity);
        protected abstract Task<T> ExecuteAddAsync(T entity, CancellationToken cancellationToken);
        protected abstract void ExecuteUpdate(T entity);
        protected abstract Task ExecuteUpdateAsync(T entity, CancellationToken cancellationToken);
        // ... 其他抽象方法

        #endregion
    }
}
