using System;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Infrastructure.Caching
{
    /// <summary>
    /// 快取服務介面
    /// </summary>
    public interface ICacheService
    {
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
