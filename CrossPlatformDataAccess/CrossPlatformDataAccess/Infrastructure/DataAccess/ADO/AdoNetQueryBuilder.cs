using System;
using System.Collections.Generic;
using System.Linq.Expression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrossPlatformDataAccess.Core.DataAccess;
using CrossPlatformDataAccess.Core.Interfaces;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.ADO
{
    /// <summary>
    /// ADO.NET 查詢建構器實作
    /// </summary>
    public class AdoNetQueryBuilder<T> : IQueryBuilder<T> where T : class
    {
        private readonly IDbProvider _provider;
        private readonly StringBuilder _query;
        private readonly List<object> _parameters;
        private readonly string _tableName;

        public AdoNetQueryBuilder(IDbProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _query = new StringBuilder();
            _parameters = new List<object>();
            _tableName = typeof(T).Name;
        }

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            // 將 Expression 轉換為 SQL WHERE 子句
            var whereClause = ExpressionToSql(predicate);
            _query.Append($" WHERE {whereClause}");
            return this;
        }

        public IQueryBuilder<T> OrWhere(Expression<Func<T, bool>> predicate)
        {
            var whereClause = ExpressionToSql(predicate);
            _query.Append($" OR {whereClause}");
            return this;
        }

        public IQueryBuilder<T> Like(string columnName, string value)
        {
            _query.Append($" {columnName} LIKE '{value}'");
            return this;
        }

        public IQueryBuilder<T> In<TValue>(Expression<Func<T, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            var propertyName = GetPropertyName(propertySelector);
            var valueList = string.Join(",", values);
            _query.Append($" {propertyName} IN ({valueList})");
            return this;
        }

        public IQueryBuilder<T> Between<TValue>(Expression<Func<T, TValue>> propertySelector, TValue start, TValue end)
        {
            var propertyName = GetPropertyName(propertySelector);
            _query.Append($" {propertyName} BETWEEN {start} AND {end}");
            return this;
        }

        public IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, bool ascending = true)
        {
            var orderByClause = ExpressionToSql(keySelector);
            _query.Append($" ORDER BY {orderByClause} {(ascending ? "ASC" : "DESC")}");
            return this;
        }

        public IQueryBuilder<T> Skip(int count)
        {
            _query.Append($" OFFSET {count} ROWS");
            return this;
        }

        public IQueryBuilder<T> Take(int count)
        {
            _query.Append($" FETCH NEXT {count} ROWS ONLY");
            return this;
        }

        public string Build()
        {
            var baseQuery = $"SELECT * FROM {_tableName}";
            return baseQuery + _query.ToString();
        }

        private string ExpressionToSql(Expression expression)
        {
            // 簡單實作，實際專案中應該使用更完整的 Expression 訪問者模式
            return expression.ToString();
        }

        private string GetPropertyName(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("Invalid expression", nameof(expression));
        }

        public void AddParameter(string name, object value)
        {
            _parameters.Add(new { Name = name, Value = value });
        }

        public IEnumerable<object> GetParameters()
        {
            return _parameters;
        }

        public IQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            _query.Append($" INCLUDE {propertyName}");
            return this;
        }

        public IQueryBuilder<T> ThenInclude<TPreviousProperty, TProperty>(Expression<Func<T, IEnumerable<TPreviousProperty>>> previousPropertySelector, Expression<Func<TPreviousProperty, TProperty>> propertySelector)
        {
            var previousPropertyName = GetPropertyName(previousPropertySelector);
            var propertyName = GetPropertyName(propertySelector);
            _query.Append($" THEN INCLUDE {previousPropertyName}.{propertyName}");
            return this;
        }

        public IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderByClause = ExpressionToSql(keySelector);
            _query.Append($" ORDER BY {orderByClause}");
            return this;
        }

        public IQueryBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderByClause = ExpressionToSql(keySelector);
            _query.Append($" ORDER BY {orderByClause} DESC");
            return this;
        }

        public IQueryBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderByClause = ExpressionToSql(keySelector);
            _query.Append($" THEN BY {orderByClause}");
            return this;
        }

        public IQueryBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderByClause = ExpressionToSql(keySelector);
            _query.Append($" THEN BY {orderByClause} DESC");
            return this;
        }

        public IQueryBuilder<T> Page(int pageNumber, int pageSize)
        {
            _query.Append($" OFFSET {pageNumber * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY");
            return this;
        }

        public List<T> ToList()
        {
            // 實作 ToList 邏輯
            return new List<T>();
        }

        public async Task<List<T>> ToListAsync(CancellationToken cancellationToken)
        {
            // 實作 ToListAsync 邏輯
            return await Task.FromResult(new List<T>());
        }

        public T FirstOrDefault()
        {
            // 實作 FirstOrDefault 邏輯
            return null;
        }

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            // 實作 FirstOrDefaultAsync 邏輯
            return await Task.FromResult(default(T));
        }

        public T SingleOrDefault()
        {
            // 實作 SingleOrDefault 邏輯
            return null;
        }

        public async Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken)
        {
            // 實作 SingleOrDefaultAsync 邏輯
            return await Task.FromResult(default(T));
        }

        public int Count()
        {
            // 實作 Count 邏輯
            return 0;
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            // 實作 CountAsync 邏輯
            return await Task.FromResult(0);
        }

        public bool Any()
        {
            // 實作 Any 邏輯
            return false;
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken)
        {
            // 實作 AnyAsync 邏輯
            return await Task.FromResult(false);
        }

        public Expression<Func<T, bool>> GetPredicate()
        {
            // 實作 GetPredicate 邏輯
            return null;
        }

        public string GetQueryString()
        {
            return _query.ToString();
        }
    }
}
