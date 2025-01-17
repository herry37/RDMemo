using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Core.DataAccess
{
    /// <summary>
    /// 基礎資料存取介面
    /// 定義基本的CRUD操作
    /// </summary>
    public interface IBaseDataAccess<T> where T : class
    {
        #region 同步操作

        /// <summary>
        /// 新增實體
        /// </summary>
        T Add(T entity);

        /// <summary>
        /// 更新實體
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// 刪除實體
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// 根據ID取得實體
        /// </summary>
        T GetById(object id);

        /// <summary>
        /// 取得所有實體
        /// </summary>
        IEnumerable<T> GetAll();

        #endregion

        #region 非同步操作

        /// <summary>
        /// 非同步新增實體
        /// </summary>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 非同步更新實體
        /// </summary>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 非同步刪除實體
        /// </summary>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 非同步根據ID取得實體
        /// </summary>
        Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 非同步取得所有實體
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
