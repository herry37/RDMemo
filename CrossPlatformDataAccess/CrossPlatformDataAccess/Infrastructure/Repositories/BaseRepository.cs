using CrossPlatformDataAccess.Core.Interfaces;
using System.Reflection;

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
            _dataAccess = dataAccess;
            _tableName = typeof(T).Name; // 假設表名與類名相同
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
        /// <returns>單一記錄</returns>
        public async Task<T> GetByIdAsync(int id)
        {
            string query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            return await _dataAccess.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
        }

        /// <summary>
        /// 新增記錄
        /// </summary>
        /// <param name="entity">要新增的記錄</param>
        /// <returns>新增的記錄</returns>
        public async Task<T> AddAsync(T entity)
        {
            var properties = GetProperties(entity);
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            string command = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
            await _dataAccess.ExecuteAsync(command, entity);
            return entity;
        }

        /// <summary>
        /// 更新記錄
        /// </summary>
        /// <param name="entity">要更新的記錄</param>
        /// <returns>更新後的記錄</returns>
        public async Task<T> UpdateAsync(T entity)
        {
            var properties = GetProperties(entity);
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

            string command = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
            await _dataAccess.ExecuteAsync(command, entity);
            return entity;
        }

        /// <summary>
        /// 刪除記錄
        /// </summary>
        /// <param name="id">要刪除的記錄的唯一標識</param>
        /// <returns>刪除的結果</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            string command = $"DELETE FROM {_tableName} WHERE Id = @Id";
            int affectedRows = await _dataAccess.ExecuteAsync(command, new { Id = id });
            return affectedRows > 0;
        }

        /// <summary>
        /// 獲取實體類型的屬性
        /// </summary>
        /// <param name="entity">實體對象</param>
        /// <returns>屬性集合</returns>
        private IEnumerable<PropertyInfo> GetProperties(T entity)
        {
            return typeof(T).GetProperties().Where(p => p.CanRead);
        }
    }
}