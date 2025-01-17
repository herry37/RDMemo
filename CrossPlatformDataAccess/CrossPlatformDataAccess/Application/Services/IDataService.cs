using System.Threading;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Application.Services
{
    /// <summary>
    /// 定義資料服務的介面
    /// </summary>
    public interface IDataService<T> where T : class
    {
        Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
