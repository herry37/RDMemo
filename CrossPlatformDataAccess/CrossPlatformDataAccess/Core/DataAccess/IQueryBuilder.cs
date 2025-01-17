using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 查詢建構器介面
    /// </summary>
    public interface IQueryBuilder<T> where T : class
    {
        #region 條件查詢

        /// <summary>
        /// 添加 Where 條件
        /// </summary>
        IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 添加 OR Where 條件
        /// </summary>
        IQueryBuilder<T> OrWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 添加 Like 條件
        /// </summary>
        IQueryBuilder<T> Like(string columnName, string value);

        /// <summary>
        /// 添加 In 條件
        /// </summary>
        IQueryBuilder<T> In<TValue>(Expression<Func<T, TValue>> propertySelector, IEnumerable<TValue> values);

        /// <summary>
        /// 添加 Between 條件
        /// </summary>
        IQueryBuilder<T> Between<TValue>(Expression<Func<T, TValue>> propertySelector, TValue start, TValue end);

        #endregion

        #region 關聯查詢

        /// <summary>
        /// 包含關聯資料
        /// </summary>
        IQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath);

        /// <summary>
        /// 包含多層關聯資料
        /// </summary>
        IQueryBuilder<T> ThenInclude<TPreviousProperty, TProperty>(
            Expression<Func<T, IEnumerable<TPreviousProperty>>> previousPropertyPath,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath);

        #endregion

        #region 排序

        /// <summary>
        /// 正向排序
        /// </summary>
        IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// 反向排序
        /// </summary>
        IQueryBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// 多重正向排序
        /// </summary>
        IQueryBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// 多重反向排序
        /// </summary>
        IQueryBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);

        #endregion

        #region 分頁

        /// <summary>
        /// 設定分頁
        /// </summary>
        IQueryBuilder<T> Page(int pageNumber, int pageSize);

        /// <summary>
        /// 設定跳過筆數
        /// </summary>
        IQueryBuilder<T> Skip(int count);

        /// <summary>
        /// 設定取得筆數
        /// </summary>
        IQueryBuilder<T> Take(int count);

        #endregion

        #region 查詢執行

        /// <summary>
        /// 執行查詢並返回結果集
        /// </summary>
        IEnumerable<T> ToList();

        /// <summary>
        /// 執行查詢並返回結果集 (非同步)
        /// </summary>
        Task<IEnumerable<T>> ToListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 返回第一筆資料
        /// </summary>
        T FirstOrDefault();

        /// <summary>
        /// 返回第一筆資料 (非同步)
        /// </summary>
        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 返回單筆資料
        /// </summary>
        T SingleOrDefault();

        /// <summary>
        /// 返回單筆資料 (非同步)
        /// </summary>
        Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 計算資料筆數
        /// </summary>
        int Count();

        /// <summary>
        /// 計算資料筆數 (非同步)
        /// </summary>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 檢查是否存在符合條件的資料
        /// </summary>
        bool Any();

        /// <summary>
        /// 檢查是否存在符合條件的資料 (非同步)
        /// </summary>
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        #endregion

        #region 查詢資訊

        /// <summary>
        /// 取得查詢條件
        /// </summary>
        Expression<Func<T, bool>> GetPredicate();

        /// <summary>
        /// 取得查詢字串
        /// </summary>
        string GetQueryString();

        #endregion
    }
}
