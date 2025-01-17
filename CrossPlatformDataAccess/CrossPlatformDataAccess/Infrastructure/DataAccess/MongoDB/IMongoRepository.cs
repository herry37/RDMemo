using System.Linq.Expressions;
using MongoDB.Driver;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.MongoDB
{
    /// <summary>
    /// MongoDB 儲存庫介面
    /// </summary>
    public interface IMongoRepository<T> where T : class
    {
        /// <summary>
        /// 取得集合
        /// </summary>
        IMongoCollection<T> Collection { get; }

        #region 同步操作

        /// <summary>
        /// 新增實體
        /// </summary>
        void Add(T entity);

        /// <summary>
        /// 批次新增實體
        /// </summary>
        void AddMany(IEnumerable<T> entities);

        /// <summary>
        /// 更新實體
        /// </summary>
        bool Update(Expression<Func<T, bool>> filter, T entity);

        /// <summary>
        /// 刪除實體
        /// </summary>
        bool Delete(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 取得單一實體
        /// </summary>
        T Get(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 取得所有實體
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        /// 依條件取得實體
        /// </summary>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 檢查是否存在
        /// </summary>
        bool Exists(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 計算數量
        /// </summary>
        long Count(Expression<Func<T, bool>> filter = null);

        #endregion

        #region 非同步操作

        /// <summary>
        /// 新增實體
        /// </summary>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批次新增實體
        /// </summary>
        Task AddManyAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新實體
        /// </summary>
        Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 刪除實體
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得單一實體
        /// </summary>
        Task<T> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得所有實體
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 依條件取得實體
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// 檢查是否存在
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// 計算數量
        /// </summary>
        Task<long> CountAsync(Expression<Func<T, bool>> filter = null, CancellationToken cancellationToken = default);

        #endregion
    }
}
