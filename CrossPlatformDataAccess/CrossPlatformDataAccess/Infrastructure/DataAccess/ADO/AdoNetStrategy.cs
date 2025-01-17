using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using CrossPlatformDataAccess.Core.DataAccess;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.ADO
{
    /// <summary>
    /// ADO.NET 實作的資料存取策略
    /// </summary>
    public class AdoNetStrategy : IDataAccessStrategy
    {
        private readonly DbConnection _connection;

        public AdoNetStrategy(DbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            AddParameters(command, parameters);

            var results = new List<T>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(MapToObject<T>(reader));
            }
            return results;
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            AddParameters(command, parameters);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class
        {
            // ADO.NET 主要使用原生 SQL，這裡可以使用 QueryBuilder 來轉換 Expression
            var builder = new AdoNetQueryBuilder<T>();
            var (sql, parameters) = builder.BuildQuery(predicate);
            return await QueryAsync<T>(sql, parameters, cancellationToken);
        }

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new AdoNetQueryBuilder<T>(_connection);
        }

        public async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _connection.BeginTransactionAsync(cancellationToken);
            return new AdoNetTransactionScope(transaction);
        }

        private void AddParameters(DbCommand command, object parameters)
        {
            if (parameters == null) return;

            var properties = parameters.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = prop.Name;
                parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        private T MapToObject<T>(DbDataReader reader)
        {
            var obj = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var columnOrdinal = reader.GetOrdinal(prop.Name);
                if (!reader.IsDBNull(columnOrdinal))
                {
                    var value = reader.GetValue(columnOrdinal);
                    prop.SetValue(obj, value);
                }
            }

            return obj;
        }
    }
}
