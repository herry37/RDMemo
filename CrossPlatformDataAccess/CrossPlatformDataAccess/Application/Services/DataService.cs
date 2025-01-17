using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrossPlatformDataAccess.Core.DataAccess;

namespace CrossPlatformDataAccess.Application.Services
{
    /// <summary>
    /// 通用資料服務實作
    /// 提供基本的CRUD操作，並可以擴展添加業務邏輯
    /// </summary>
    public class DataService<T> : IDataService<T> where T : class
    {
        private readonly IDataAccess<T> _dataAccess;

        public DataService(IDataAccess<T> dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public virtual async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            // 這裡可以添加業務邏輯，例如：權限檢查、快取處理等
            return await _dataAccess.GetByIdAsync(id, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // 這裡可以添加業務邏輯，例如：分頁、過濾、排序等
            return await _dataAccess.GetAllAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            // 這裡可以添加業務邏輯，例如：資料驗證、前置處理等
            return await _dataAccess.AddAsync(entity, cancellationToken);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            // 這裡可以添加業務邏輯，例如：並發檢查、修改記錄等
            await _dataAccess.UpdateAsync(entity, cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            // 這裡可以添加業務邏輯，例如：關聯檢查、軟刪除等
            await _dataAccess.DeleteAsync(entity, cancellationToken);
        }
    }
}
