using System.Data;

namespace CrossPlatformDataAccess.Core.Interfaces
{
    /// <summary>
    /// 定義資料庫工廠的介面，用於創建資料庫連線
    /// </summary>
    public interface IDbFactory
    {
        /// <summary>
        /// 創建資料庫連線
        /// </summary>
        /// <returns>資料庫連線物件</returns>
        IDbConnection CreateConnection();
    }
}
