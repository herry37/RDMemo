using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 統一的資料存取介面
    /// 整合同步和非同步操作
    /// </summary>
    public interface IDataAccess<T> where T : class
    {
        #region 基本CRUD操作

        // 同步操作
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        T GetById(object id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        // 非同步操作
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        #endregion

        #region SQL操作

        // 同步SQL操作
        IEnumerable<T> QueryWithSql(string sql, object parameters = null);
        int ExecuteSql(string sql, object parameters = null);

        // 非同步SQL操作
        Task<IEnumerable<T>> QueryWithSqlAsync(string sql, object parameters = null, CancellationToken cancellationToken = default);
        Task<int> ExecuteSqlAsync(string sql, object parameters = null, CancellationToken cancellationToken = default);

        #endregion

        #region 交易操作

        // 同步交易
        TResult ExecuteInTransaction<TResult>(Func<TResult> operation);
        void ExecuteInTransaction(Action operation);

        // 非同步交易
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

        #endregion

        #region 查詢建構器

        // 取得查詢建構器
        IQueryBuilder<T> Query();

        #endregion
    }
}
