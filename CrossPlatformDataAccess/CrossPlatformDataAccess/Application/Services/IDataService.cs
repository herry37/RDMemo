using System.Threading;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Application.Services
{
    /// <summary>
    ///  定義資料服務的介面
    /// </summary>
    /// <typeparam name="T">資料模型</typeparam>
    public interface IDataService<T> where T : class
    {
        /// <summary>
        /// 依據ID取得資料
        /// </summary>
        /// <param name="id">資料ID</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>資料</returns>
        Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>資料集合</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="entity">資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>資料</returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="entity">資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>資料</returns>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="entity">資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
