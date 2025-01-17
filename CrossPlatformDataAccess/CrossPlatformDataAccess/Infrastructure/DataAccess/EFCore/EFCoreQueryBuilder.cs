using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrossPlatformDataAccess.Core.DataAccess;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.EFCore
{
    /// <summary>
    /// EF Core 查詢建構器實作
    /// </summary>
    public class EFCoreQueryBuilder<T> : IQueryBuilder<T> where T : class
    {
        private readonly DbContext _context;
        private IQueryable<T> _query;
        private Expression<Func<T, bool>> _predicate;
        private int? _pageNumber;
        private int? _pageSize;

        public EFCoreQueryBuilder(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _query = _context.Set<T>();
        }

        #region 條件查詢實作

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            _predicate = _predicate != null 
                ? CombineExpressions(_predicate, predicate, ExpressionType.AndAlso)
                : predicate;
            _query = _query.Where(predicate);
            return this;
        }

        public IQueryBuilder<T> OrWhere(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            _predicate = _predicate != null
                ? CombineExpressions(_predicate, predicate, ExpressionType.OrElse)
                : predicate;
            _query = _query.Where(predicate);
            return this;
        }

        public IQueryBuilder<T> Like(string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException(nameof(columnName));

            // 使用 EF.Functions.Like 進行模糊查詢
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, columnName);
            var likeMethod = typeof(EF.Functions).GetMethod("Like", new[] { typeof(string), typeof(string) });
            var efFunctions = Expression.Property(null, typeof(EF).GetProperty("Functions"));
            var likeCall = Expression.Call(efFunctions, likeMethod, property, Expression.Constant($"%{value}%"));
            var lambda = Expression.Lambda<Func<T, bool>>(likeCall, parameter);

            return Where(lambda);
        }

        public IQueryBuilder<T> In<TValue>(Expression<Func<T, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            if (propertySelector == null)
                throw new ArgumentNullException(nameof(propertySelector));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var valuesArray = values.ToArray();
            if (!valuesArray.Any())
                return this;

            var parameter = propertySelector.Parameters[0];
            var memberExpression = propertySelector.Body;
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TValue));

            var valuesExpression = Expression.Constant(valuesArray);
            var containsCall = Expression.Call(null, containsMethod, valuesExpression, memberExpression);
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            return Where(lambda);
        }

        public IQueryBuilder<T> Between<TValue>(Expression<Func<T, TValue>> propertySelector, TValue start, TValue end) where TValue : IComparable<TValue>
        {
            if (propertySelector == null)
                throw new ArgumentNullException(nameof(propertySelector));

            var parameter = propertySelector.Parameters[0];
            var memberExpression = propertySelector.Body;

            // 建立 >= start 條件
            var greaterEqual = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(start));
            // 建立 <= end 條件
            var lessEqual = Expression.LessThanOrEqual(memberExpression, Expression.Constant(end));
            // 組合條件
            var andExpression = Expression.AndAlso(greaterEqual, lessEqual);
            var lambda = Expression.Lambda<Func<T, bool>>(andExpression, parameter);

            return Where(lambda);
        }

        #endregion

        #region 關聯查詢實作

        public IQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            if (navigationPropertyPath == null)
                throw new ArgumentNullException(nameof(navigationPropertyPath));

            _query = _query.Include(navigationPropertyPath);
            return this;
        }

        public IQueryBuilder<T> ThenInclude<TPreviousProperty, TProperty>(
            Expression<Func<T, IEnumerable<TPreviousProperty>>> previousPropertyPath,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        {
            if (previousPropertyPath == null)
                throw new ArgumentNullException(nameof(previousPropertyPath));
            if (navigationPropertyPath == null)
                throw new ArgumentNullException(nameof(navigationPropertyPath));

            _query = _query.Include(previousPropertyPath).ThenInclude(navigationPropertyPath);
            return this;
        }

        #endregion

        #region 排序實作

        public IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            _query = _query.OrderBy(keySelector);
            return this;
        }

        public IQueryBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            _query = _query.OrderByDescending(keySelector);
            return this;
        }

        public IQueryBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            _query = ((IOrderedQueryable<T>)_query).ThenBy(keySelector);
            return this;
        }

        public IQueryBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            _query = ((IOrderedQueryable<T>)_query).ThenByDescending(keySelector);
            return this;
        }

        #endregion

        #region 分頁實作

        public IQueryBuilder<T> Page(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            _pageNumber = pageNumber;
            _pageSize = pageSize;
            ApplyPaging();
            return this;
        }

        public IQueryBuilder<T> Skip(int count)
        {
            if (count < 0)
                throw new ArgumentException("Skip count must be greater than or equal to 0", nameof(count));

            _query = _query.Skip(count);
            return this;
        }

        public IQueryBuilder<T> Take(int count)
        {
            if (count < 0)
                throw new ArgumentException("Take count must be greater than or equal to 0", nameof(count));

            _query = _query.Take(count);
            return this;
        }

        #endregion

        #region 查詢執行實作

        public IEnumerable<T> ToList()
        {
            ApplyQueryConditions();
            return _query.ToList();
        }

        public async Task<IEnumerable<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            ApplyQueryConditions();
            return await _query.ToListAsync(cancellationToken);
        }

        public T FirstOrDefault()
        {
            ApplyQueryConditions();
            return _query.FirstOrDefault();
        }

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            ApplyQueryConditions();
            return await _query.FirstOrDefaultAsync(cancellationToken);
        }

        public T SingleOrDefault()
        {
            ApplyQueryConditions();
            return _query.SingleOrDefault();
        }

        public async Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            ApplyQueryConditions();
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

        #region 查詢資訊實作

        public Expression<Func<T, bool>> GetPredicate()
        {
            return _predicate;
        }

        public string GetQueryString()
        {
            ApplyQueryConditions();
            return _query.ToQueryString();
        }

        #endregion

        #region 私有輔助方法

        /// <summary>
        /// 建立分頁資訊
        /// </summary>
        private void ApplyPaging()
        {
            if (_pageNumber.HasValue && _pageSize.HasValue)
            {
                _query = _query
                    .Skip((_pageNumber.Value - 1) * _pageSize.Value)
                    .Take(_pageSize.Value);
            }
        }

        /// <summary>
        /// 檢查並套用所有查詢條件
        /// </summary>
        private void ApplyQueryConditions()
        {
            // 在這裡可以添加其他查詢條件的處理邏輯
            ApplyPaging();
        }

        /// <summary>
        /// 組合兩個表達式
        /// </summary>
        private Expression<Func<T, bool>> CombineExpressions(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2,
            ExpressionType type)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.MakeBinary(type, left, right),
                parameter);
        }

        #endregion
    }

    /// <summary>
    /// 用於替換表達式中的參數
    /// </summary>
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}
