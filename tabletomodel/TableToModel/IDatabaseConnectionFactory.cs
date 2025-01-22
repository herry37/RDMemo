using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableToModel
{
    /// <summary>
    /// 資料庫連接工廠介面，定義了創建資料庫連接和生成連接字串的方法
    /// </summary>
    public interface IDatabaseConnectionFactory
    {
        /// <summary>
        /// 創建資料庫連接
        /// </summary>
        /// <param name="connectionString">資料庫連接字串</param>
        /// <returns>資料庫連接實例</returns>
        IDbConnection CreateConnection(string connectionString);
        /// <summary>
        /// 生成資料庫連接字串
        /// </summary>
        /// <param name="IP">資料庫伺服器的 IP 地址</param>
        /// <param name="ID">資料庫用戶名</param>
        /// <param name="Pass">資料庫用戶密碼</param>
        /// <param name="Catalog">資料庫名稱</param>
        /// <returns>資料庫連接字串</returns>
        string GetConnectionString(string IP, string ID, string Pass, string Catalog);
    }
}
