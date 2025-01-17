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
            _context = context;
            _query = _context.Set<T>();
        }

        #region 查詢條件實作

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate;
            _query = _query.Where(predicate);
            return this;
        }

        public IQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            _query = _query.Include(navigationPropertyPath);
            return this;
        }

        public IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, bool ascending = true)
        {
            _query = ascending 
                ? _query.OrderBy(keySelector)
                : _query.OrderByDescending(keySelector);
            return this;
        }

        public IQueryBuilder<T> Page(int pageNumber, int pageSize)
        {
            _pageNumber = pageNumber;
            _pageSize = pageSize;
            _query = _query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return this;
        }

        #endregion

        #region 同步執行方法實作

        public IEnumerable<T> ToList()
        {
            return _query.ToList();
        }

        public T FirstOrDefault()
        {
            return _query.FirstOrDefault();
        }

        public int Count()
        {
            return _query.Count();
        }

        #endregion

        #region 非同步執行方法實作

        public async Task<IEnumerable<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await _query.ToListAsync(cancellationToken);
        }

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await _query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _query.CountAsync(cancellationToken);
        }

        #endregion

        #region 查詢資訊實作

        public Expression<Func<T, bool>> GetPredicate()
        {
            return _predicate;
        }

        public string GetQueryString()
        {
            // 使用 EF Core 的查詢追蹤功能取得 SQL
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

        #endregion
    }
}
