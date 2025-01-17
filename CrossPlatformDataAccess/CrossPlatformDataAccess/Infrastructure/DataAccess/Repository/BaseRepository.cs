using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CrossPlatformDataAccess.Core.DataAccess;
using CrossPlatformDataAccess.Core.Logging;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.Repository
{
    /// <summary>
    /// 基礎儲存庫實作，同時支援同步和非同步操作
    /// </summary>
    public abstract class BaseRepository<T> : IBaseDataAccess<T>, IBaseDataAccessAsync<T> where T : class
    {
        protected readonly IDataAccessStrategy _strategy;
        protected readonly ILogger _logger;

        protected BaseRepository(IDataAccessStrategy strategy, ILogger logger)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region 同步操作
        public virtual T Add(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                _logger.LogInformation($"開始新增 {typeof(T).Name}");
                using var transaction = _strategy.BeginTransaction();
                var result = ExecuteAdd(entity);
                transaction.Commit();
                _logger.LogInformation($"成功新增 {typeof(T).Name}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"新增 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                _logger.LogInformation($"開始更新 {typeof(T).Name}");
                using var transaction = _strategy.BeginTransaction();
                ExecuteUpdate(entity);
                transaction.Commit();
                _logger.LogInformation($"成功更新 {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"更新 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                _logger.LogInformation($"開始刪除 {typeof(T).Name}");
                using var transaction = _strategy.BeginTransaction();
                ExecuteDelete(entity ?? throw new ArgumentNullException(nameof(entity)));
                transaction.Commit();
                _logger.LogInformation($"成功刪除 {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"刪除 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            try
            {
                _logger.LogInformation($"開始查詢 {typeof(T).Name}");
                var result = ExecuteGet(predicate);
                _logger.LogInformation($"成功查詢 {typeof(T).Name}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"查詢 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual IEnumerable<T> QueryWithSql(string sql, object parameters = null)
        {
            ArgumentNullException.ThrowIfNull(sql);
            try
            {
                _logger.LogInformation($"開始執行SQL查詢: {sql}");
                var result = _strategy.Query<T>(sql, parameters);
                _logger.LogInformation("SQL查詢執行成功");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL查詢失敗: {sql}", ex);
                throw;
            }
        }

        public virtual int ExecuteSql(string sql, object parameters = null)
        {
            ArgumentNullException.ThrowIfNull(sql);
            try
            {
                _logger.LogInformation($"開始執行SQL命令: {sql}");
                var result = _strategy.Execute(sql, parameters);
                _logger.LogInformation("SQL命令執行成功");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL命令執行失敗: {sql}", ex);
                throw;
            }
        }

        public virtual TResult ExecuteInTransaction<TResult>(Func<TResult> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
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

        public virtual void ExecuteInTransaction(Action operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
            using var transaction = _strategy.BeginTransaction();
            try
            {
                _logger.LogInformation("開始執行交易");
                operation();
                transaction.Commit();
                _logger.LogInformation("交易執行成功");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("交易執行失敗", ex);
                throw;
            }
        }
        #endregion

        #region 非同步操作
        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                _logger.LogInformation($"開始新增 {typeof(T).Name}");
                using var transaction = await _strategy.BeginTransactionAsync(cancellationToken);
                var result = await ExecuteAddAsync(entity, cancellationToken);
                await transaction.CommitAsync();
                _logger.LogInformation($"成功新增 {typeof(T).Name}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"新增 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                _logger.LogInformation($"開始更新 {typeof(T).Name}");
                using var transaction = await _strategy.BeginTransactionAsync(cancellationToken);
                await ExecuteUpdateAsync(entity, cancellationToken);
                await transaction.CommitAsync();
                _logger.LogInformation($"成功更新 {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"更新 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                _logger.LogInformation($"開始刪除 {typeof(T).Name}");
                using var transaction = await _strategy.BeginTransactionAsync(cancellationToken);
                await ExecuteDeleteAsync(entity, cancellationToken);
                await transaction.CommitAsync();
                _logger.LogInformation($"成功刪除 {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"刪除 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            try
            {
                _logger.LogInformation($"開始查詢 {typeof(T).Name}");
                var result = await ExecuteGetAsync(predicate, cancellationToken);
                _logger.LogInformation($"成功查詢 {typeof(T).Name}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"查詢 {typeof(T).Name} 失敗", ex);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> QueryWithSqlAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(sql);
            try
            {
                _logger.LogInformation($"開始執行SQL查詢: {sql}");
                var result = await _strategy.QueryAsync<T>(sql, parameters, cancellationToken);
                _logger.LogInformation("SQL查詢執行成功");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL查詢失敗: {sql}", ex);
                throw;
            }
        }

        public virtual async Task<int> ExecuteSqlAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(sql);
            try
            {
                _logger.LogInformation($"開始執行SQL命令: {sql}");
                var result = await _strategy.ExecuteAsync(sql, parameters, cancellationToken);
                _logger.LogInformation("SQL命令執行成功");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL命令執行失敗: {sql}", ex);
                throw;
            }
        }

        public virtual async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);
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

        public virtual async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);
            using var transaction = await _strategy.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogInformation("開始執行交易");
                await operation();
                await transaction.CommitAsync();
                _logger.LogInformation("交易執行成功");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("交易執行失敗", ex);
                throw;
            }
        }
        #endregion

        #region 需要被實作的抽象方法
        protected abstract T ExecuteAdd(T entity);
        protected abstract void ExecuteUpdate(T entity);
        protected abstract void ExecuteDelete(T entity);
        protected abstract IEnumerable<T> ExecuteGet(Expression<Func<T, bool>> predicate);

        protected abstract Task<T> ExecuteAddAsync(T entity, CancellationToken cancellationToken);
        protected abstract Task ExecuteUpdateAsync(T entity, CancellationToken cancellationToken);
        protected abstract Task ExecuteDeleteAsync(T entity, CancellationToken cancellationToken);
        protected abstract Task<IEnumerable<T>> ExecuteGetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        #endregion
    }
}
