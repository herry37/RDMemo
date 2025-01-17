using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrossPlatformDataAccess.Core.DataAccess;
using CrossPlatformDataAccess.Core.Logging;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.EFCore
{
    /// <summary>
    /// EF Core資料存取策略實作
    /// </summary>
    public class EFCoreDataAccessStrategy : IDataAccessStrategy
    {
        private readonly DbContext _context;
        private readonly ILogger _logger;

        public EFCoreDataAccessStrategy(DbContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region 連線管理

        public DbConnection GetConnection()
        {
            return _context.Database.GetDbConnection();
        }

        public void OpenConnection()
        {
            var connection = GetConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public async Task OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = GetConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }
        }

        #endregion

        #region 交易管理

        public ITransactionScope BeginTransaction()
        {
            return new EFCoreTransactionScope(_context);
        }

        public async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new EFCoreTransactionScope(_context));
        }

        #endregion

        #region 查詢操作

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new EFCoreQueryBuilder<T>(_context);
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null) where T : class
        {
            try
            {
                _logger.LogInformation($"執行SQL查詢: {sql}");
                return _context.Set<T>().FromSqlRaw(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL查詢失敗: {sql}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                _logger.LogInformation($"執行SQL查詢: {sql}");
                return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL查詢失敗: {sql}", ex);
                throw;
            }
        }

        public int Execute(string sql, object parameters = null)
        {
            try
            {
                _logger.LogInformation($"執行SQL命令: {sql}");
                return _context.Database.ExecuteSqlRaw(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL命令執行失敗: {sql}", ex);
                throw;
            }
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"執行SQL命令: {sql}");
                return await _context.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL命令執行失敗: {sql}", ex);
                throw;
            }
        }

        #endregion
    }

    /// <summary>
    /// EF Core交易範圍實作
    /// </summary>
    internal class EFCoreTransactionScope : ITransactionScope
    {
        private readonly DbContext _context;
        private readonly IDbContextTransaction _transaction;
        private bool _disposed;

        public EFCoreTransactionScope(DbContext context)
        {
            _context = context;
            _transaction = context.Database.BeginTransaction();
        }

        public void Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EFCoreTransactionScope));

            _transaction.Commit();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EFCoreTransactionScope));

            await _transaction.CommitAsync(cancellationToken);
        }

        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EFCoreTransactionScope));

            _transaction.Rollback();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EFCoreTransactionScope));

            await _transaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction.Dispose();
                _disposed = true;
            }
        }
    }
}
