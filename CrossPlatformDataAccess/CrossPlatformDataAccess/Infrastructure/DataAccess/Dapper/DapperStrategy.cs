using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using CrossPlatformDataAccess.Core.DataAccess;
using CrossPlatformDataAccess.Core.Interfaces;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.Dapper
{
    /// <summary>
    /// Dapper 實作的資料存取策略
    /// </summary>
    public class DapperStrategy : IDataAccessStrategy
    {
        private readonly IDbProvider _provider;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="provider">資料庫提供者</param>
        public DapperStrategy(IDbProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// 取得資料庫提供者
        /// </summary>
        public IDbProvider Provider => _provider;

        #region SQL 操作

        /// <summary>
        /// 執行 SQL 查詢並返回實體列表 (同步)
        /// </summary>
        public IEnumerable<T> Query<T>(string sql, object parameters = null) where T : class
        {
            using var connection = _provider.CreateConnection();
            return connection.Query<T>(sql, parameters);
        }

        /// <summary>
        /// 執行 SQL 查詢並返回實體列表 (非同步)
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = _provider.CreateConnection();
            return await connection.QueryAsync<T>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// 執行 SQL 命令 (同步)
        /// </summary>
        public int Execute(string sql, object parameters = null)
        {
            using var connection = _provider.CreateConnection();
            return connection.Execute(sql, parameters);
        }

        /// <summary>
        /// 執行 SQL 命令 (非同步)
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            using var connection = _provider.CreateConnection();
            return await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }

        #endregion

        #region CRUD 操作

        /// <summary>
        /// 新增實體 (同步)
        /// </summary>
        public T Add<T>(T entity) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var tableName = typeof(T).Name;
            var properties = entity.GetType().GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            
            var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters}); SELECT SCOPE_IDENTITY();";
            
            using var connection = _provider.CreateConnection();
            var id = connection.ExecuteScalar<int>(sql, entity);
            
            // 設置 ID 屬性
            var idProperty = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            if (idProperty != null)
            {
                idProperty.SetValue(entity, id);
            }
            
            return entity;
        }

        /// <summary>
        /// 新增實體 (非同步)
        /// </summary>
        public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var tableName = typeof(T).Name;
            var properties = entity.GetType().GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            
            var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters}); SELECT SCOPE_IDENTITY();";
            
            using var connection = _provider.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
            
            // 設置 ID 屬性
            var idProperty = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            if (idProperty != null)
            {
                idProperty.SetValue(entity, id);
            }
            
            return entity;
        }

        /// <summary>
        /// 更新實體 (同步)
        /// </summary>
        public bool Update<T>(T entity) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var tableName = typeof(T).Name;
            var properties = entity.GetType().GetProperties();
            var setClause = string.Join(", ", properties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                                                     .Select(p => $"{p.Name} = @{p.Name}"));
            
            var sql = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            var rowsAffected = connection.Execute(sql, entity);
            return rowsAffected > 0;
        }

        /// <summary>
        /// 更新實體 (非同步)
        /// </summary>
        public async Task<bool> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var tableName = typeof(T).Name;
            var properties = entity.GetType().GetProperties();
            var setClause = string.Join(", ", properties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                                                     .Select(p => $"{p.Name} = @{p.Name}"));
            
            var sql = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
            return rowsAffected > 0;
        }

        /// <summary>
        /// 刪除實體 (同步)
        /// </summary>
        public bool Delete<T>(T entity) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var tableName = typeof(T).Name;
            var sql = $"DELETE FROM {tableName} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            var rowsAffected = connection.Execute(sql, entity);
            return rowsAffected > 0;
        }

        /// <summary>
        /// 刪除實體 (非同步)
        /// </summary>
        public async Task<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var tableName = typeof(T).Name;
            var sql = $"DELETE FROM {tableName} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
            return rowsAffected > 0;
        }

        /// <summary>
        /// 根據 ID 取得實體 (同步)
        /// </summary>
        public T GetById<T>(object id) where T : class
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            
            var tableName = typeof(T).Name;
            var sql = $"SELECT * FROM {tableName} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            return connection.QueryFirstOrDefault<T>(sql, new { Id = id });
        }

        /// <summary>
        /// 根據 ID 取得實體 (非同步)
        /// </summary>
        public async Task<T> GetByIdAsync<T>(object id, CancellationToken cancellationToken = default) where T : class
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            
            var tableName = typeof(T).Name;
            var sql = $"SELECT * FROM {tableName} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// 取得所有實體 (同步)
        /// </summary>
        public IEnumerable<T> GetAll<T>() where T : class
        {
            var tableName = typeof(T).Name;
            var sql = $"SELECT * FROM {tableName}";
            
            using var connection = _provider.CreateConnection();
            return connection.Query<T>(sql);
        }

        /// <summary>
        /// 取得所有實體 (非同步)
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            var tableName = typeof(T).Name;
            var sql = $"SELECT * FROM {tableName}";
            
            using var connection = _provider.CreateConnection();
            return await connection.QueryAsync<T>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        #endregion

        #region 交易管理

        /// <summary>
        /// 開始交易 (同步)
        /// </summary>
        public IDbTransaction BeginTransaction()
        {
            var connection = _provider.CreateConnection();
            connection.Open();
            return connection.BeginTransaction();
        }

        /// <summary>
        /// 開始交易 (非同步)
        /// </summary>
        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var connection = _provider.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            return connection.BeginTransaction();
        }

        /// <summary>
        /// 在交易中執行操作 (同步)
        /// </summary>
        public T ExecuteInTransaction<T>(Func<T> operation)
        {
            using var connection = _provider.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
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

        /// <summary>
        /// 在交易中執行操作 (非同步)
        /// </summary>
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            using var connection = _provider.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = await operation();
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        #endregion

        #region 批次操作

        /// <summary>
        /// 批次新增實體 (同步)
        /// </summary>
        public void BulkInsert<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) return;

            var tableName = typeof(T).Name;
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            
            var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";
            
            using var connection = _provider.CreateConnection();
            connection.Execute(sql, entities);
        }

        /// <summary>
        /// 批次新增實體 (非同步)
        /// </summary>
        public async Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) return;

            var tableName = typeof(T).Name;
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            
            var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";
            
            using var connection = _provider.CreateConnection();
            await connection.ExecuteAsync(new CommandDefinition(sql, entities, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// 批次更新實體 (同步)
        /// </summary>
        public void BulkUpdate<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) return;

            var tableName = typeof(T).Name;
            var properties = typeof(T).GetProperties();
            var setClause = string.Join(", ", properties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                                                     .Select(p => $"{p.Name} = @{p.Name}"));
            
            var sql = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            connection.Execute(sql, entities);
        }

        /// <summary>
        /// 批次更新實體 (非同步)
        /// </summary>
        public async Task BulkUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) return;

            var tableName = typeof(T).Name;
            var properties = typeof(T).GetProperties();
            var setClause = string.Join(", ", properties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                                                     .Select(p => $"{p.Name} = @{p.Name}"));
            
            var sql = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            await connection.ExecuteAsync(new CommandDefinition(sql, entities, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// 批次刪除實體 (同步)
        /// </summary>
        public void BulkDelete<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) return;

            var tableName = typeof(T).Name;
            var sql = $"DELETE FROM {tableName} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            connection.Execute(sql, entities);
        }

        /// <summary>
        /// 批次刪除實體 (非同步)
        /// </summary>
        public async Task BulkDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any()) return;

            var tableName = typeof(T).Name;
            var sql = $"DELETE FROM {tableName} WHERE Id = @Id";
            
            using var connection = _provider.CreateConnection();
            await connection.ExecuteAsync(new CommandDefinition(sql, entities, cancellationToken: cancellationToken));
        }

        #endregion

        #region 預存程序

        /// <summary>
        /// 執行預存程序 (同步)
        /// </summary>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, object parameters = null) where T : class
        {
            if (string.IsNullOrEmpty(procedureName)) throw new ArgumentNullException(nameof(procedureName));
            
            using var connection = _provider.CreateConnection();
            return connection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// 執行預存程序 (非同步)
        /// </summary>
        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrEmpty(procedureName)) throw new ArgumentNullException(nameof(procedureName));
            
            using var connection = _provider.CreateConnection();
            return await connection.QueryAsync<T>(new CommandDefinition(procedureName, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        }

        #endregion

        #region 查詢建構

        /// <summary>
        /// 建立查詢建構器
        /// </summary>
        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new DapperQueryBuilder<T>(_provider.CreateConnection());
        }

        #endregion
    }
}
