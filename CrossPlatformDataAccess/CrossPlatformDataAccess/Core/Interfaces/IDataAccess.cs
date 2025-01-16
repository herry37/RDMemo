using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CrossPlatformDataAccess.Core.Interfaces
{
    /// <summary>
    /// 定義數據存取的介面，支援同步和非同步操作
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// 執行查詢並返回結果集
        /// </summary>
        /// <typeparam name="T">結果類型</typeparam>
        /// <param name="query">SQL 查詢語句</param>
        /// <returns>查詢結果集</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string query);

        /// <summary>
        /// 執行查詢並返回結果集
        /// </summary>
        /// <typeparam name="T">結果類型</typeparam>
        /// <param name="query">SQL 查詢語句</param>
        /// <param name="parameters">查詢的參數</param>
        /// <returns>查詢結果集</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters);

        /// <summary>
        /// 執行命令並返回受影響的行數
        /// </summary>
        /// <param name="command">SQL 命令語句</param>
        /// <returns>受影響的行數</returns>
        Task<int> ExecuteAsync(string command);

        /// <summary>
        /// 執行命令並返回受影響的行數
        /// </summary>
        /// <param name="command">SQL 命令語句</param>
        /// <param name="parameters">命令的參數</param>
        /// <returns>受影響的行數</returns>
        Task<int> ExecuteAsync(string command, object parameters);

        /// <summary>
        /// 執行查詢並返回單一或預設結果
        /// </summary>
        /// <typeparam name="T">結果類型</typeparam>
        /// <param name="query">SQL 查詢語句</param>
        /// <param name="parameters">查詢的參數</param>
        /// <returns>單一或預設結果</returns>
        Task<T> QuerySingleOrDefaultAsync<T>(string query, object parameters);
        /// <summary>
        /// 执行查询，返回DataSet
        /// </summary>
        /// <param name="query">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>DataSet</returns>
        Task<DataSet> ExecuteQueryAsync(string query, SqlParameter[] parameters);
        /// <summary>
        /// 执行EF查询，返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="query">查询函数</param>
        /// <returns>IEnumerable<T></returns>
        Task<IEnumerable<T>> EFQueryAsync<T>(Func<DbContext, IQueryable<T>> query) where T : class;
        /// <summary>
        /// 执行EF命令，返回int
        /// </summary>
        /// <param name="command">命令函数</param>
        /// <returns>int</returns>
        Task<int> EFExecuteAsync(Func<DbContext, Task<int>> command);
    }
}