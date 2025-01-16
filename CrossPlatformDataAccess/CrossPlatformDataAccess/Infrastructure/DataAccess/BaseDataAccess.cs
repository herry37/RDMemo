using CrossPlatformDataAccess.Core.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess
{
    /// <summary>
    /// 基礎數據存取實作，支援同步和非同步操作
    /// </summary>
    public class BaseDataAccess : IDataAccess
    {
        private readonly IDbFactory _dbFactory;

        /// <summary>
        /// 建構子，接受資料庫工廠的注入
        /// </summary>
        /// <param name="dbFactory">資料庫工廠</param>
        public BaseDataAccess(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// 執行查詢並返回結果集
        /// </summary>
        /// <typeparam name="T">結果類型</typeparam>
        /// <param name="query">SQL 查詢語句</param>
        /// <returns>查詢結果集</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string query)
        {
            using (var connection = _dbFactory.CreateConnection())
            {
                return await connection.QueryAsync<T>(query);
            }
        }

        /// <summary>
        /// 執行查詢並返回結果集
        /// </summary>
        /// <typeparam name="T">結果類型</typeparam>
        /// <param name="query">SQL 查詢語句</param>
        /// <param name="parameters">查詢的參數</param>
        /// <returns>查詢結果集</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters)
        {
            using (var connection = _dbFactory.CreateConnection())
            {
                return await connection.QueryAsync<T>(query, parameters);
            }
        }

        /// <summary>
        /// 執行命令並返回受影響的行數
        /// </summary>
        /// <param name="command">SQL 命令語句</param>
        /// <returns>受影響的行數</returns>
        public async Task<int> ExecuteAsync(string command)
        {
            using (var connection = _dbFactory.CreateConnection())
            {
                return await connection.ExecuteAsync(command);
            }
        }

        /// <summary>
        /// 執行命令並返回受影響的行數
        /// </summary>
        /// <param name="command">SQL 命令語句</param>
        /// <param name="parameters">命令的參數</param>
        /// <returns>受影響的行數</returns>
        public async Task<int> ExecuteAsync(string command, object parameters)
        {
            using (var connection = _dbFactory.CreateConnection())
            {
                return await connection.ExecuteAsync(command, parameters);
            }
        }

        /// <summary>
        /// 執行查詢並返回單一或預設結果
        /// </summary>
        /// <typeparam name="T">結果類型</typeparam>
        /// <param name="query">SQL 查詢語句</param>
        /// <param name="parameters">查詢的參數</param>
        /// <returns>單一或預設結果</returns>
        public async Task<T> QuerySingleOrDefaultAsync<T>(string query, object parameters)
        {
            using (var connection = _dbFactory.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<T>(query, parameters);
            }
        }
        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <param name="query">查詢語句</param>
        /// <param name="parameters">參數</param>
        /// <returns>查詢結果</returns>
        public async Task<DataSet> ExecuteQueryAsync(string query, SqlParameter[] parameters)
        {
            // 創建數據庫連接
            using (var connection = _dbFactory.CreateConnection())
            {
                // 創建SqlCommand對象
                using (var command = new SqlCommand(query, (SqlConnection)connection))
                {
                    // 如果參數不為空，則添加參數
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    // 創建SqlDataAdapter對象
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        // 創建DataSet对象
                        DataSet dataSet = new DataSet();
                        // 異步填充DataSet
                        await Task.Run(() => adapter.Fill(dataSet));
                        // 返回DataSet
                        return dataSet;
                    }
                }
            }
        }

        // 异步执行EF查询，返回IEnumerable<T>
        public Task<IEnumerable<T>> EFQueryAsync<T>(Func<Microsoft.EntityFrameworkCore.DbContext, IQueryable<T>> query) where T : class
        {
            // 未实现
            throw new NotImplementedException();
        }

        // 异步执行EF命令，返回int
        public Task<int> EFExecuteAsync(Func<Microsoft.EntityFrameworkCore.DbContext, Task<int>> command)
        {
            // 未实现
            throw new NotImplementedException();
        }
    }
}