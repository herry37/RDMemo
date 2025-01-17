using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Dapper;
using CrossPlatformDataAccess.Core.DataAccess;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.Dapper
{
    /// <summary>
    /// Dapper 實作的資料存取策略
    /// </summary>
    public class DapperStrategy : IDataAccessStrategy
    {
        private readonly IDbConnection _connection;

        public DapperStrategy(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            return await _connection.QueryAsync<T>(sql, parameters);
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            return await _connection.ExecuteAsync(sql, parameters);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class
        {
            // Dapper 主要使用原生 SQL，這裡可以使用 DapperQueryBuilder 來轉換 Expression
            var builder = new DapperQueryBuilder<T>();
            var (sql, parameters) = builder.BuildQuery(predicate);
            return await _connection.QueryAsync<T>(sql, parameters);
        }

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new DapperQueryBuilder<T>(_connection);
        }

        public async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = _connection.BeginTransaction();
            return new DapperTransactionScope(transaction);
        }
    }
}
