using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 查詢建構器介面
    /// 提供流暢的API來建構複雜查詢
    /// </summary>
    public interface IQueryBuilder<T> where T : class
    {
        #region 查詢條件

        /// <summary>
        /// 添加WHERE條件
        /// </summary>
        /// <param name="predicate">查詢條件表達式</param>
        IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 添加關聯資料查詢（適用於EF Core的Include）
        /// </summary>
        /// <param name="navigationPropertyPath">關聯屬性路徑</param>
        IQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath);
        
        /// <summary>
        /// 添加排序條件
        /// </summary>
        /// <param name="keySelector">排序欄位選擇器</param>
        /// <param name="ascending">是否升序（預設為true）</param>
        IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, bool ascending = true);
        
        /// <summary>
        /// 添加分頁
        /// </summary>
        /// <param name="pageNumber">頁碼（從1開始）</param>
        /// <param name="pageSize">每頁筆數</param>
        IQueryBuilder<T> Page(int pageNumber, int pageSize);

        #endregion

        #region 執行方法

        /// <summary>
        /// 執行查詢並返回結果集
        /// </summary>
        IEnumerable<T> ToList();
        
        /// <summary>
        /// 執行查詢並返回第一筆符合條件的資料
        /// </summary>
        T FirstOrDefault();
        
        /// <summary>
        /// 取得符合條件的資料筆數
        /// </summary>
        int Count();

        /// <summary>
        /// 非同步執行查詢並返回結果集
        /// </summary>
        Task<IEnumerable<T>> ToListAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步執行查詢並返回第一筆符合條件的資料
        /// </summary>
        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步取得符合條件的資料筆數
        /// </summary>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        #endregion

        #region 查詢資訊

        /// <summary>
        /// 取得目前的查詢條件
        /// </summary>
        Expression<Func<T, bool>> GetPredicate();

        /// <summary>
        /// 取得完整的查詢語句（如果資料庫提供者支援）
        /// </summary>
        string GetQueryString();

        #endregion
    }
}
