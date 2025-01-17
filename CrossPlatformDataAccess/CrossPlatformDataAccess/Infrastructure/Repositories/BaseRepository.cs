using CrossPlatformDataAccess.Core.Interfaces;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Infrastructure.Repositories
{
    /// <summary>
    /// 基礎儲存庫實作，提供通用的數據操作方法
    /// </summary>
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly IDataAccess _dataAccess;
        private readonly string _tableName;

        /// <summary>
        /// 建構子，接受數據存取層的注入
        /// </summary>
        /// <param name="dataAccess">數據存取層</param>
        public BaseRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _tableName = typeof(T).Name;
        }

        /// <summary>
        /// 獲取所有記錄
        /// </summary>
        /// <returns>所有記錄的集合</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string query = $"SELECT * FROM {_tableName}";
            return await _dataAccess.QueryAsync<T>(query);
        }

        /// <summary>
        /// 根據 ID 獲取單一記錄
        /// </summary>
        /// <param name="id">記錄的唯一標識</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>單一記錄</returns>
        public async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);
            string query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            var parameters = new { Id = id };
            var result = await _dataAccess.QueryFirstOrDefaultAsync<T>(query, parameters, cancellationToken);
            return result;
        }

        /// <summary>
        /// 新增記錄
        /// </summary>
        /// <param name="entity">要新增的記錄</param>
        public async Task AddAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var properties = GetProperties(entity);
            var columns = string.Join(", ", properties.Keys);
            var values = string.Join(", ", properties.Keys.Select(k => "@" + k));
            string query = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
            await _dataAccess.ExecuteAsync(query, entity);
        }

        /// <summary>
        /// 更新記錄
        /// </summary>
        /// <param name="entity">要更新的記錄</param>
        public async Task UpdateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var properties = GetProperties(entity);
            var setClause = string.Join(", ", properties.Keys.Select(k => $"{k} = @{k}"));
            string query = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
            await _dataAccess.ExecuteAsync(query, entity);
        }

        /// <summary>
        /// 刪除記錄
        /// </summary>
        /// <param name="id">要刪除的記錄ID</param>
        public async Task DeleteAsync(int id)
        {
            string query = $"DELETE FROM {_tableName} WHERE Id = @Id";
            var parameters = new { Id = id };
            await _dataAccess.ExecuteAsync(query, parameters);
        }

        /// <summary>
        /// 獲取實體類型的屬性
        /// </summary>
        /// <param name="entity">實體對象</param>
        /// <returns>屬性集合</returns>
        private Dictionary<string, object> GetProperties(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return entity.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .ToDictionary(p => p.Name, p => p.GetValue(entity));
        }
    }
}