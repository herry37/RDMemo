using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 統一的資料存取介面，整合同步和非同步操作
    /// </summary>
    /// <typeparam name="T">資料實體類型</typeparam>
    public interface IDataAccess<T> where T : class
    {
        #region 基本CRUD操作

        // 同步操作
        /// <summary>
        /// 新增資料實體
        /// </summary>
        /// <param name="entity">要新增的資料實體</param>
        /// <returns>新增的資料實體</returns>
        T Add(T entity);
        
        /// <summary>
        /// 更新資料實體
        /// </summary>
        /// <param name="entity">要更新的資料實體</param>
        void Update(T entity);
        
        /// <summary>
        /// 刪除資料實體
        /// </summary>
        /// <param name="entity">要刪除的資料實體</param>
        void Delete(T entity);
        
        /// <summary>
        /// 獲取所有資料實體
        /// </summary>
        /// <returns>所有資料實體的集合</returns>
        IEnumerable<T> GetAll();
        
        /// <summary>
        /// 根據ID獲取資料實體
        /// </summary>
        /// <param name="id">資料實體的ID</param>
        /// <returns>對應的資料實體</returns>
        T? GetById(object id);
        
        /// <summary>
        /// 根據條件查找資料實體
        /// </summary>
        /// <param name="predicate">查詢條件</param>
        /// <returns>符合條件的資料實體集合</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        // 非同步操作
        /// <summary>
        /// 非同步新增資料實體
        /// </summary>
        /// <param name="entity">要新增的資料實體</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>新增的資料實體</returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步更新資料實體
        /// </summary>
        /// <param name="entity">要更新的資料實體</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步刪除資料實體
        /// </summary>
        /// <param name="entity">要刪除的資料實體</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步獲取所有資料實體
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>所有資料實體的集合</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步根據ID獲取資料實體
        /// </summary>
        /// <param name="id">資料實體的ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>對應的資料實體</returns>
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步根據條件查找資料實體
        /// </summary>
        /// <param name="predicate">查詢條件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>符合條件的資料實體集合</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        #endregion

        #region SQL操作

        // 同步SQL操作
        /// <summary>
        /// 執行SQL查詢
        /// </summary>
        /// <param name="sql">SQL查詢語句</param>
        /// <param name="parameters">查詢參數</param>
        /// <returns>查詢結果</returns>
        IEnumerable<T> QueryWithSql(string sql, object? parameters = null);
        
        /// <summary>
        /// 執行SQL命令
        /// </summary>
        /// <param name="sql">SQL命令語句</param>
        /// <param name="parameters">命令參數</param>
        /// <returns>命令執行結果</returns>
        int ExecuteSql(string sql, object? parameters = null);

        // 非同步SQL操作
        /// <summary>
        /// 非同步執行SQL查詢
        /// </summary>
        /// <param name="sql">SQL查詢語句</param>
        /// <param name="parameters">查詢參數</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>查詢結果</returns>
        Task<IEnumerable<T>> QueryWithSqlAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步執行SQL命令
        /// </summary>
        /// <param name="sql">SQL命令語句</param>
        /// <param name="parameters">命令參數</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>命令執行結果</returns>
        Task<int> ExecuteSqlAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        #endregion

        #region 交易操作

        // 同步交易
        /// <summary>
        /// 執行交易操作
        /// </summary>
        /// <typeparam name="TResult">交易結果類型</typeparam>
        /// <param name="operation">交易操作</param>
        /// <returns>交易結果</returns>
        TResult ExecuteInTransaction<TResult>(Func<TResult> operation);
        
        /// <summary>
        /// 執行交易操作
        /// </summary>
        /// <param name="operation">交易操作</param>
        void ExecuteInTransaction(Action operation);

        // 非同步交易
        /// <summary>
        /// 非同步執行交易操作
        /// </summary>
        /// <typeparam name="TResult">交易結果類型</typeparam>
        /// <param name="operation">交易操作</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>交易結果</returns>
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 非同步執行交易操作
        /// </summary>
        /// <param name="operation">交易操作</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

        #endregion

        #region 查詢建構器

        // 取得查詢建構器
        /// <summary>
        /// 取得查詢建構器
        /// </summary>
        /// <returns>查詢建構器</returns>
        IQueryBuilder<T> Query();

        #endregion
    }
}
