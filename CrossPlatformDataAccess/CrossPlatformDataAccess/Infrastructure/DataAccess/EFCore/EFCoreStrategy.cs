using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using CrossPlatformDataAccess.Core.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.EFCore
{
    /// <summary>
    /// EF Core 實作的資料存取策略
    /// </summary>
    public class EFCoreStrategy : IDataAccessStrategy
    {
        private readonly DbContext _context;

        public EFCoreStrategy(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IDbProvider Provider => throw new NotImplementedException("EF Core does not use IDbProvider");

        #region SQL 操作

        public IEnumerable<T> Query<T>(string sql, object? parameters = null) where T : class
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            return _context.Set<T>().FromSqlRaw(sql, parameters ?? Array.Empty<object>()).ToList();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            return await _context.Set<T>().FromSqlRaw(sql, parameters ?? Array.Empty<object>()).ToListAsync(cancellationToken);
        }

        public int Execute(string sql, object? parameters = null)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            return _context.Database.ExecuteSqlRaw(sql, parameters ?? Array.Empty<object>());
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            return await _context.Database.ExecuteSqlRawAsync(sql, parameters ?? Array.Empty<object>(), cancellationToken);
        }

        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, object? parameters = null) where T : class
        {
            if (string.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException(nameof(procedureName));

            return _context.Set<T>().FromSqlRaw($"EXEC {procedureName}", parameters ?? Array.Empty<object>()).ToList();
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException(nameof(procedureName));

            return await _context.Set<T>().FromSqlRaw($"EXEC {procedureName}", parameters ?? Array.Empty<object>()).ToListAsync(cancellationToken);
        }

        #endregion

        #region CRUD 操作

        public T Add<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Set<T>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public bool Update<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Update(entity);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public bool Delete<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public T GetById<T>(object id) where T : class
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync<T>(object id, CancellationToken cancellationToken = default) where T : class
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _context.Set<T>().FindAsync(new[] { id }, cancellationToken);
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            return _context.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        #endregion

        #region 查詢建構

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new EFCoreQueryBuilder<T>(_context);
        }

        #endregion

        #region 交易管理

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();
            return new EFCoreTransactionWrapper(transaction);
        }

        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new EFCoreTransactionWrapper(transaction);
        }

        public T ExecuteInTransaction<T>(Func<T> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var result = operation();
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await operation();
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        #endregion

        #region 批次操作

        public void BulkInsert<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Set<T>().AddRange(entities);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public void BulkUpdate<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Set<T>().UpdateRange(entities);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task BulkUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Set<T>().UpdateRange(entities);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public void BulkDelete<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Set<T>().RemoveRange(entities);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task BulkDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        #endregion
    }

    /// <summary>
    /// EF Core 交易包裝器，將 IDbContextTransaction 轉換為 IDbTransaction
    /// </summary>
    internal class EFCoreTransactionWrapper : IDbTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EFCoreTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public IDbConnection Connection => throw new NotImplementedException();

        public IsolationLevel IsolationLevel => _transaction.GetDbTransaction().IsolationLevel;

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
