using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using CrossPlatformDataAccess.Core.Logging;
using CrossPlatformDataAccess.Core.DataAccess;
using CrossPlatformDataAccess.Infrastructure.DataAccess.Base;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.Repository
{
    /// <summary>
    /// 通用儲存庫實作
    /// </summary>
    public class GenericRepository<T> : BaseDataAccess where T : class
    {
        public GenericRepository(IDataAccessStrategy strategy, ILogger logger)
            : base(strategy, logger)
        {
        }

        /// <summary>
        /// 新增實體
        /// </summary>
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return await ExecuteInTransactionAsync(async () =>
            {
                var sql = GenerateInsertSql(entity);
                await _strategy.ExecuteAsync(sql, entity, cancellationToken);
                return entity;
            }, $"新增 {typeof(T).Name}", cancellationToken);
        }

        /// <summary>
        /// 更新實體
        /// </summary>
        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await ExecuteInTransactionAsync(async () =>
            {
                var sql = GenerateUpdateSql(entity);
                await _strategy.ExecuteAsync(sql, entity, cancellationToken);
            }, $"更新 {typeof(T).Name}", cancellationToken);
        }

        /// <summary>
        /// 刪除實體
        /// </summary>
        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await ExecuteInTransactionAsync(async () =>
            {
                var sql = GenerateDeleteSql(entity);
                await _strategy.ExecuteAsync(sql, entity, cancellationToken);
            }, $"刪除 {typeof(T).Name}", cancellationToken);
        }

        /// <summary>
        /// 根據條件查詢
        /// </summary>
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
         
            ArgumentNullException.ThrowIfNull(predicate);


            if (_strategy == null)
            {
                throw new ArgumentNullException(nameof(_strategy));
            }
            return await ExecuteQueryAsync(async () =>
            {
                return await _strategy.QueryAsync(predicate, cancellationToken);
            }, $"查詢 {typeof(T).Name}", cancellationToken);
        }

        /// <summary>
        /// 使用原生SQL查詢
        /// </summary>
        public async Task<IEnumerable<T>> QueryWithSqlAsync(
            string sql,
            object parameters = null!,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            if (_strategy == null)
            {
                throw new ArgumentNullException(nameof(_strategy));
            }
            return await ExecuteQueryAsync(async () =>
            {
                return await _strategy.QueryAsync<T>(sql, parameters, cancellationToken);
            }, $"SQL查詢 {typeof(T).Name}", cancellationToken);
        }

        /// <summary>
        /// 取得查詢建構器
        /// </summary>
        public IQueryBuilder<T> Query()
        {
            if (_strategy == null)
            {
                throw new ArgumentNullException(nameof(_strategy));
            }
            return _strategy.Query<T>();
        }

        #region Helper Methods
        private string GenerateInsertSql(T entity)
        {
            // 這裡實作根據實體生成 INSERT SQL
            // 可以使用反射或 Expression 來生成
            throw new NotImplementedException();
        }

        private string GenerateUpdateSql(T entity)
        {
            // 這裡實作根據實體生成 UPDATE SQL
            throw new NotImplementedException();
        }

        private string GenerateDeleteSql(T entity)
        {
            // 這裡實作根據實體生成 DELETE SQL
            throw new NotImplementedException();
        }
        #endregion
    }
}
