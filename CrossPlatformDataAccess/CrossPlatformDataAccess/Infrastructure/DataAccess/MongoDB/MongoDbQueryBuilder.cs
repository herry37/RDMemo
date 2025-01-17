using System.Linq.Expressions;
using CrossPlatformDataAccess.Core.DataAccess;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.MongoDB
{
    /// <summary>
    /// MongoDB 查詢建構器實作
    /// </summary>
    public class MongoDbQueryBuilder<T> : IQueryBuilder<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;
        private IMongoQueryable<T> _query;
        private Expression<Func<T, bool>> _filter;

        public MongoDbQueryBuilder(IMongoCollection<T> collection)
        {
            _collection = collection;
            _query = collection.AsQueryable();
        }

        #region Filtering

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (_filter == null)
                _filter = predicate;
            else
                _filter = Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(_filter.Body, Expression.Invoke(predicate, _filter.Parameters)),
                    _filter.Parameters);

            _query = _query.Where(predicate);
            return this;
        }

        public IQueryBuilder<T> OrWhere(Expression<Func<T, bool>> predicate)
        {
            if (_filter == null)
                _filter = predicate;
            else
                _filter = Expression.Lambda<Func<T, bool>>(
                    Expression.OrElse(_filter.Body, Expression.Invoke(predicate, _filter.Parameters)),
                    _filter.Parameters);

            _query = _collection.AsQueryable().Where(_filter);
            return this;
        }

        public IQueryBuilder<T> Like(string columnName, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, columnName);
            var constant = Expression.Constant(value, typeof(string));
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var expression = Expression.Lambda<Func<T, bool>>(
                Expression.Call(property, method, constant),
                parameter);

            return Where(expression);
        }

        public IQueryBuilder<T> In<TValue>(Expression<Func<T, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var memberExpression = Expression.Invoke(propertySelector, parameter);
            var constant = Expression.Constant(values);
            var method = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TValue));
            var expression = Expression.Lambda<Func<T, bool>>(
                Expression.Call(method, constant, memberExpression),
                parameter);

            return Where(expression);
        }

        public IQueryBuilder<T> Between<TValue>(Expression<Func<T, TValue>> propertySelector, TValue start, TValue end) where TValue : IComparable<TValue>
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Invoke(propertySelector, parameter);
            var startConstant = Expression.Constant(start);
            var endConstant = Expression.Constant(end);
            var greaterThan = Expression.GreaterThanOrEqual(property, startConstant);
            var lessThan = Expression.LessThanOrEqual(property, endConstant);
            var andExpression = Expression.AndAlso(greaterThan, lessThan);
            var expression = Expression.Lambda<Func<T, bool>>(andExpression, parameter);

            return Where(expression);
        }

        #endregion

        #region Sorting

        public IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _query = _query.OrderBy(keySelector);
            return this;
        }

        public IQueryBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _query = _query.OrderByDescending(keySelector);
            return this;
        }

        public IQueryBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (_query is IOrderedMongoQueryable<T> orderedQuery)
                _query = orderedQuery.ThenBy(keySelector);
            else
                _query = _query.OrderBy(keySelector);
            return this;
        }

        public IQueryBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (_query is IOrderedMongoQueryable<T> orderedQuery)
                _query = orderedQuery.ThenByDescending(keySelector);
            else
                _query = _query.OrderByDescending(keySelector);
            return this;
        }

        #endregion

        #region Paging

        public IQueryBuilder<T> Page(int pageNumber, int pageSize)
        {
            _query = _query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return this;
        }

        public IQueryBuilder<T> Skip(int count)
        {
            _query = _query.Skip(count);
            return this;
        }

        public IQueryBuilder<T> Take(int count)
        {
            _query = _query.Take(count);
            return this;
        }

        #endregion

        #region Execution

        public IEnumerable<T> ToList()
        {
            return _query.ToList();
        }

        public async Task<IEnumerable<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await _query.ToListAsync(cancellationToken);
        }

        public T FirstOrDefault()
        {
            return _query.FirstOrDefault();
        }

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await _query.FirstOrDefaultAsync(cancellationToken);
        }

        public T SingleOrDefault()
        {
            return _query.SingleOrDefault();
        }

        public async Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await _query.SingleOrDefaultAsync(cancellationToken);
        }

        public int Count()
        {
            return _query.Count();
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _query.CountAsync(cancellationToken);
        }

        public bool Any()
        {
            return _query.Any();
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await _query.AnyAsync(cancellationToken);
        }

        #endregion

        #region Query Information

        public Expression<Func<T, bool>> GetPredicate()
        {
            return _filter;
        }

        public string GetQueryString()
        {
            return _query.ToString();
        }

        #endregion
    }
}
