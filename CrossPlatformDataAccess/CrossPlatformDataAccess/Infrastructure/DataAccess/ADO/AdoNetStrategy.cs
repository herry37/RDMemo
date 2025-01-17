using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using Dapper;
using CrossPlatformDataAccess.Core.DataAccess;
using CrossPlatformDataAccess.Infrastructure.DataAccess.ADO; // 確保引入 TransactionExtensions

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.ADO
{
    public class AdoNetStrategy : IDataAccessStrategy
    {
        private readonly DbConnection _connection;
        private readonly ILogger<AdoNetStrategy> _logger;
        private readonly IDbProvider _provider;

        private static readonly Action<ILogger, string, Exception?> LogQuery =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1, nameof(LogQuery)),
                "Executing: {Operation}");

        private static readonly Action<ILogger, string, string, Exception?> LogError =
            LoggerMessage.Define<string, string>(
                LogLevel.Error,
                new EventId(2, nameof(LogError)),
                "{Operation} failed: {Message}");

        private static readonly Action<ILogger, string, string, Exception?> LogTransaction =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(3, nameof(LogTransaction)),
                "Transaction {Operation}: {Message}");

        public AdoNetStrategy(DbConnection connection, ILogger<AdoNetStrategy> logger, IDbProvider provider)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IDbProvider Provider => _provider;

        public IEnumerable<T> Query<T>(string sql, object? parameters = null) where T : class
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            try
            {
                LogQuery(_logger, $"Query<{typeof(T).Name}>", null);
                OpenConnectionIfClosed();
                return _connection.Query<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                LogError(_logger, "Query", ex.Message, ex);
                throw;
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            try
            {
                LogQuery(_logger, $"QueryAsync<{typeof(T).Name}>", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                var queryResult = await _connection.QueryAsync<T>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)).ConfigureAwait(false);
                return queryResult;
            }
            catch (Exception ex)
            {
                LogError(_logger, "QueryAsync", ex.Message, ex);
                throw;
            }
        }

        public int Execute(string sql, object? parameters = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            try
            {
                LogQuery(_logger, "Execute", null);
                OpenConnectionIfClosed();
                return _connection.Execute(sql, parameters);
            }
            catch (Exception ex)
            {
                LogError(_logger, "Execute", ex.Message, ex);
                throw;
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(sql);
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            try
            {
                LogQuery(_logger, "ExecuteAsync", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                return await _connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogError(_logger, "ExecuteAsync", ex.Message, ex);
                throw;
            }
        }

        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, object? parameters = null) where T : class
        {
            ArgumentException.ThrowIfNullOrEmpty(procedureName);
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            try
            {
                LogQuery(_logger, $"StoredProcedure: {procedureName}", null);
                OpenConnectionIfClosed();
                return _connection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                LogError(_logger, "StoredProcedure", ex.Message, ex);
                throw;
            }
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentException.ThrowIfNullOrEmpty(procedureName);
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            try
            {
                LogQuery(_logger, $"StoredProcedureAsync: {procedureName}", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                return await _connection.QueryAsync<T>(new CommandDefinition(procedureName, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogError(_logger, "StoredProcedureAsync", ex.Message, ex);
                throw;
            }
        }

        public T Add<T>(T entity) where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            var sql = GenerateInsertSql<T>();
            try
            {
                LogQuery(_logger, $"Add<{typeof(T).Name}>", null);
                OpenConnectionIfClosed();
                var id = _connection.ExecuteScalar(sql, entity);
                SetEntityId(entity, id);
                return entity;
            }
            catch (Exception ex)
            {
                LogError(_logger, "Add", ex.Message, ex);
                throw;
            }
        }

        public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            var sql = GenerateInsertSql<T>();
            try
            {
                LogQuery(_logger, $"AddAsync<{typeof(T).Name}>", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                var id = await _connection.ExecuteScalarAsync(new CommandDefinition(sql, entity, cancellationToken: cancellationToken)).ConfigureAwait(false);
                SetEntityId(entity, id);
                return entity;
            }
            catch (Exception ex)
            {
                LogError(_logger, "AddAsync", ex.Message, ex);
                throw;
            }
        }

        public bool Update<T>(T entity) where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            var sql = GenerateUpdateSql<T>();
            try
            {
                LogQuery(_logger, $"Update<{typeof(T).Name}>", null);
                OpenConnectionIfClosed();
                return _connection.Execute(sql, entity) > 0;
            }
            catch (Exception ex)
            {
                LogError(_logger, "Update", ex.Message, ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            var sql = GenerateUpdateSql<T>();
            try
            {
                LogQuery(_logger, $"UpdateAsync<{typeof(T).Name}>", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                var result = await _connection.ExecuteAsync(new CommandDefinition(sql, entity, cancellationToken: cancellationToken)).ConfigureAwait(false);
                return result > 0;
            }
            catch (Exception ex)
            {
                LogError(_logger, "UpdateAsync", ex.Message, ex);
                throw;
            }
        }

        public bool Delete<T>(T entity) where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            var sql = GenerateDeleteSql<T>();
            try
            {
                LogQuery(_logger, $"Delete<{typeof(T).Name}>", null);
                OpenConnectionIfClosed();
                return _connection.Execute(sql, entity) > 0;
            }
            catch (Exception ex)
            {
                LogError(_logger, "Delete", ex.Message, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            var sql = GenerateDeleteSql<T>();
            try
            {
                LogQuery(_logger, $"DeleteAsync<{typeof(T).Name}>", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                var result = await _connection.ExecuteAsync(new CommandDefinition(sql, entity, cancellationToken: cancellationToken)).ConfigureAwait(false);
                return result > 0;
            }
            catch (Exception ex)
            {
                LogError(_logger, "DeleteAsync", ex.Message, ex);
                throw;
            }
        }

        public T GetById<T>(object id) where T : class
        {
            ArgumentNullException.ThrowIfNull(id);
            var sql = GenerateGetByIdSql<T>();
            try
            {
                LogQuery(_logger, $"GetById<{typeof(T).Name}>", null);
                OpenConnectionIfClosed();
                return _connection.QueryFirstOrDefault<T>(sql, new { Id = id }) ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with id {id} not found");
            }
            catch (Exception ex)
            {
                LogError(_logger, "GetById", ex.Message, ex);
                throw;
            }
        }

        public async Task<T> GetByIdAsync<T>(object id, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(id);
            var sql = GenerateGetByIdSql<T>();
            try
            {
                LogQuery(_logger, $"GetByIdAsync<{typeof(T).Name}>", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                var result = await _connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)).ConfigureAwait(false);
                return result ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with id {id} not found");
            }
            catch (Exception ex)
            {
                LogError(_logger, "GetByIdAsync", ex.Message, ex);
                throw;
            }
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            var sql = GenerateGetAllSql<T>();
            try
            {
                LogQuery(_logger, $"GetAll<{typeof(T).Name}>", null);
                OpenConnectionIfClosed();
                return _connection.Query<T>(sql);
            }
            catch (Exception ex)
            {
                LogError(_logger, "GetAll", ex.Message, ex);
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            var sql = GenerateGetAllSql<T>();
            try
            {
                LogQuery(_logger, $"GetAllAsync<{typeof(T).Name}>", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                return await _connection.QueryAsync<T>(new CommandDefinition(sql, cancellationToken: cancellationToken)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogError(_logger, "GetAllAsync", ex.Message, ex);
                throw;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            try
            {
                LogTransaction(_logger, "Begin", "Starting new transaction", null);
                OpenConnectionIfClosed();
                return _connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                LogError(_logger, "BeginTransaction", ex.Message, ex);
                throw;
            }
        }

        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                LogTransaction(_logger, "BeginAsync", "Starting new transaction", null);
                await OpenConnectionIfClosedAsync(cancellationToken).ConfigureAwait(false);
                return await Task.Run(() => _connection.BeginTransaction(), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogError(_logger, "BeginTransactionAsync", ex.Message, ex);
                throw;
            }
        }

        public T ExecuteInTransaction<T>(Func<T> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
            using var transaction = BeginTransaction();
            try
            {
                LogTransaction(_logger, "Execute", "Executing operation in transaction", null);
                var result = operation();
                transaction.CommitAsync().Wait(); // 更新 Commit 呼叫
                LogTransaction(_logger, "Commit", "Transaction committed successfully", null);
                return result;
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "Rollback", "Rolling back transaction due to error", null);
                transaction.RollbackAsync().Wait(); // 更新 Rollback 呼叫
                LogError(_logger, "ExecuteInTransaction", ex.Message, ex);
                throw;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);
            using var transaction = await BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                LogTransaction(_logger, "ExecuteAsync", "Executing async operation in transaction", null);
                var result = await operation().ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false); // 更新 CommitAsync 呼叫
                LogTransaction(_logger, "CommitAsync", "Transaction committed successfully", null);
                return result;
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "RollbackAsync", "Rolling back transaction due to error", null);
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false); // 更新 RollbackAsync 呼叫
                LogError(_logger, "ExecuteInTransactionAsync", ex.Message, ex);
                throw;
            }
        }

        public void BulkInsert<T>(IEnumerable<T> entities) where T : class
        {
            ArgumentNullException.ThrowIfNull(entities);
            using var transaction = BeginTransaction();
            try
            {
                LogTransaction(_logger, "BulkInsert", $"Starting bulk insert for {typeof(T).Name}", null);
                foreach (var entity in entities)
                {
                    Add(entity);
                }
                transaction.CommitAsync().Wait(); // 更新 Commit 呼叫
                LogTransaction(_logger, "BulkInsert", "Bulk insert completed successfully", null);
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "BulkInsert", "Rolling back bulk insert", null);
                transaction.RollbackAsync().Wait(); // 更新 Rollback 呼叫
                LogError(_logger, "BulkInsert", ex.Message, ex);
                throw;
            }
        }

        public async Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(entities);
            using var transaction = await BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                LogTransaction(_logger, "BulkInsertAsync", $"Starting bulk insert for {typeof(T).Name}", null);
                foreach (var entity in entities)
                {
                    await AddAsync(entity, cancellationToken).ConfigureAwait(false);
                }
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false); // 更新 CommitAsync 呼叫
                LogTransaction(_logger, "BulkInsertAsync", "Bulk insert completed successfully", null);
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "BulkInsertAsync", "Rolling back bulk insert", null);
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false); // 更新 RollbackAsync 呼叫
                LogError(_logger, "BulkInsertAsync", ex.Message, ex);
                throw;
            }
        }

        public void BulkUpdate<T>(IEnumerable<T> entities) where T : class
        {
            ArgumentNullException.ThrowIfNull(entities);
            using var transaction = BeginTransaction();
            try
            {
                LogTransaction(_logger, "BulkUpdate", $"Starting bulk update for {typeof(T).Name}", null);
                foreach (var entity in entities)
                {
                    Update(entity);
                }
                transaction.CommitAsync().Wait(); // 更新 Commit 呼叫
                LogTransaction(_logger, "BulkUpdate", "Bulk update completed successfully", null);
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "BulkUpdate", "Rolling back bulk update", null);
                transaction.RollbackAsync().Wait(); // 更新 Rollback 呼叫
                LogError(_logger, "BulkUpdate", ex.Message, ex);
                throw;
            }
        }

        public async Task BulkUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(entities);
            using var transaction = await BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                LogTransaction(_logger, "BulkUpdateAsync", $"Starting bulk update for {typeof(T).Name}", null);
                foreach (var entity in entities)
                {
                    await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
                }
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false); // 更新 CommitAsync 呼叫
                LogTransaction(_logger, "BulkUpdateAsync", "Bulk update completed successfully", null);
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "BulkUpdateAsync", "Rolling back bulk update", null);
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false); // 更新 RollbackAsync 呼叫
                LogError(_logger, "BulkUpdateAsync", ex.Message, ex);
                throw;
            }
        }

        public void BulkDelete<T>(IEnumerable<T> entities) where T : class
        {
            ArgumentNullException.ThrowIfNull(entities);
            using var transaction = BeginTransaction();
            try
            {
                LogTransaction(_logger, "BulkDelete", $"Starting bulk delete for {typeof(T).Name}", null);
                foreach (var entity in entities)
                {
                    Delete(entity);
                }
                transaction.CommitAsync().Wait(); // 更新 Commit 呼叫
                LogTransaction(_logger, "BulkDelete", "Bulk delete completed successfully", null);
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "BulkDelete", "Rolling back bulk delete", null);
                transaction.RollbackAsync().Wait(); // 更新 Rollback 呼叫
                LogError(_logger, "BulkDelete", ex.Message, ex);
                throw;
            }
        }

        public async Task BulkDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(entities);
            using var transaction = await BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                LogTransaction(_logger, "BulkDeleteAsync", $"Starting bulk delete for {typeof(T).Name}", null);
                foreach (var entity in entities)
                {
                    await DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
                }
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false); // 更新 CommitAsync 呼叫
                LogTransaction(_logger, "BulkDeleteAsync", "Bulk delete completed successfully", null);
            }
            catch (Exception ex)
            {
                LogTransaction(_logger, "BulkDeleteAsync", "Rolling back bulk delete", null);
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false); // 更新 RollbackAsync 呼叫
                LogError(_logger, "BulkDeleteAsync", ex.Message, ex);
                throw;
            }
        }

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new AdoNetQueryBuilder<T>(_connection, _provider);
        }

        private void OpenConnectionIfClosed()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        private async Task OpenConnectionIfClosedAsync(CancellationToken cancellationToken)
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static string GenerateInsertSql<T>()
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            return $"INSERT INTO {type.Name} ({columns}) VALUES ({parameters})";
        }

        private static string GenerateUpdateSql<T>()
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.Name != "Id");
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            return $"UPDATE {type.Name} SET {setClause} WHERE Id = @Id";
        }

        private static string GenerateDeleteSql<T>()
        {
            var type = typeof(T);
            return $"DELETE FROM {type.Name} WHERE Id = @Id";
        }

        private static string GenerateGetByIdSql<T>()
        {
            var type = typeof(T);
            return $"SELECT * FROM {type.Name} WHERE Id = @Id";
        }

        private static string GenerateGetAllSql<T>()
        {
            var type = typeof(T);
            return $"SELECT * FROM {type.Name}";
        }

        private static void SetEntityId<T>(T entity, object? id) where T : class
        {
            if (id == null) return;

            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(entity, Convert.ChangeType(id, idProperty.PropertyType, CultureInfo.InvariantCulture));
            }
        }
    }
}
