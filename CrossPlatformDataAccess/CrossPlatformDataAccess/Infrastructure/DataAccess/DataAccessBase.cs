using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CrossPlatformDataAccess.Core.DataAccess;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess
{
    /// <summary>
    /// 資料存取基礎類別，提供所有資料存取操作的基本實作
    /// </summary>
    /// <typeparam name="T">資料實體類型</typeparam>
    public abstract class DataAccessBase<T> : IDataAccess<T> where T : class
    {
        private readonly IDataAccessStrategy _strategy;
        private readonly ILogger<DataAccessBase<T>> _logger;

        protected DataAccessBase(IDataAccessStrategy strategy, ILogger<DataAccessBase<T>> logger)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual T Add(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return ExecuteInTransaction(() =>
            {
                _logger.LogInformation("Adding {EntityType}", typeof(T).Name);
                return ExecuteAdd(entity);
            });
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return await ExecuteInTransactionAsync(async () =>
            {
                _logger.LogInformation("Adding {EntityType} asynchronously", typeof(T).Name);
                return await ExecuteAddAsync(entity, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public virtual void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ExecuteInTransaction(() =>
            {
                _logger.LogInformation("Updating {EntityType}", typeof(T).Name);
                ExecuteUpdate(entity);
            });
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await ExecuteInTransactionAsync(async () =>
            {
                _logger.LogInformation("Updating {EntityType} asynchronously", typeof(T).Name);
                await ExecuteUpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public virtual void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ExecuteInTransaction(() =>
            {
                _logger.LogInformation("Deleting {EntityType}", typeof(T).Name);
                ExecuteDelete(entity);
            });
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await ExecuteInTransactionAsync(async () =>
            {
                _logger.LogInformation("Deleting {EntityType} asynchronously", typeof(T).Name);
                await ExecuteDeleteAsync(entity, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<T> GetAll()
        {
            _logger.LogInformation("Getting all {EntityType}", typeof(T).Name);
            return ExecuteGetAll();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting all {EntityType} asynchronously", typeof(T).Name);
            return await ExecuteGetAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual T? GetById(object id)
        {
            ArgumentNullException.ThrowIfNull(id);
            _logger.LogInformation("Getting {EntityType} by ID", typeof(T).Name);
            return ExecuteGetById(id);
        }

        public virtual async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);
            _logger.LogInformation("Getting {EntityType} by ID asynchronously", typeof(T).Name);
            return await ExecuteGetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            _logger.LogInformation("Finding {EntityType} by predicate", typeof(T).Name);
            return ExecuteFind(predicate);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            _logger.LogInformation("Finding {EntityType} by predicate asynchronously", typeof(T).Name);
            return await ExecuteFindAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<T> QueryWithSql(string sql, object? parameters = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            _logger.LogInformation("Executing SQL query for {EntityType}", typeof(T).Name);
            return _strategy.Query<T>(sql, parameters);
        }

        public virtual async Task<IEnumerable<T>> QueryWithSqlAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            _logger.LogInformation("Executing SQL query for {EntityType} asynchronously", typeof(T).Name);
            return await _strategy.QueryAsync<T>(sql, parameters, cancellationToken).ConfigureAwait(false);
        }

        public virtual int ExecuteSql(string sql, object? parameters = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            _logger.LogInformation("Executing SQL command");
            return _strategy.Execute(sql, parameters);
        }

        public virtual async Task<int> ExecuteSqlAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            _logger.LogInformation("Executing SQL command asynchronously");
            return await _strategy.ExecuteAsync(sql, parameters, cancellationToken).ConfigureAwait(false);
        }

        public virtual TResult ExecuteInTransaction<TResult>(Func<TResult> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
            using var transaction = _strategy.BeginTransaction();
            try
            {
                _logger.LogInformation("Beginning transaction");
                var result = operation();
                transaction.Commit();
                _logger.LogInformation("Transaction committed");
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Transaction rolled back due to error");
                throw;
            }
        }

        public virtual void ExecuteInTransaction(Action operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
            using var transaction = _strategy.BeginTransaction();
            try
            {
                _logger.LogInformation("Beginning transaction");
                operation();
                transaction.Commit();
                _logger.LogInformation("Transaction committed");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Transaction rolled back due to error");
                throw;
            }
        }

        public virtual async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);
            var transaction = await _strategy.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                _logger.LogInformation("Beginning transaction asynchronously");
                var result = await operation().ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Transaction committed");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError(ex, "Transaction rolled back due to error");
                throw;
            }
        }

        public virtual async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);
            var transaction = await _strategy.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                _logger.LogInformation("Beginning transaction asynchronously");
                await operation().ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Transaction committed");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError(ex, "Transaction rolled back due to error");
                throw;
            }
        }

        public virtual IQueryBuilder<T> Query()
        {
            return _strategy.Query<T>();
        }

        // Abstract methods that must be implemented by derived classes
        protected abstract T ExecuteAdd(T entity);
        protected abstract Task<T> ExecuteAddAsync(T entity, CancellationToken cancellationToken);
        protected abstract void ExecuteUpdate(T entity);
        protected abstract Task ExecuteUpdateAsync(T entity, CancellationToken cancellationToken);
        protected abstract void ExecuteDelete(T entity);
        protected abstract Task ExecuteDeleteAsync(T entity, CancellationToken cancellationToken);
        protected abstract IEnumerable<T> ExecuteGetAll();
        protected abstract Task<IEnumerable<T>> ExecuteGetAllAsync(CancellationToken cancellationToken);
        protected abstract T? ExecuteGetById(object id);
        protected abstract Task<T?> ExecuteGetByIdAsync(object id, CancellationToken cancellationToken);
        protected abstract IEnumerable<T> ExecuteFind(Expression<Func<T, bool>> predicate);
        protected abstract Task<IEnumerable<T>> ExecuteFindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    }
}
