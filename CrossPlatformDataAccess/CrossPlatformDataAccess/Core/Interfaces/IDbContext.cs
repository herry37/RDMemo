using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace CrossPlatformDataAccess.Core.Interfaces
{
    /// <summary>
    /// 定義資料庫上下文的介面，提供資料庫操作的基本功能
    /// </summary>
    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// 取得資料庫實體，提供對資料庫層級操作的存取
        /// </summary>
        DatabaseFacade Database { get; }

        /// <summary>
        /// 取得資料庫連線物件，用於直接存取資料庫連線
        /// </summary>
        /// <returns>資料庫連線物件</returns>
        DbConnection GetConnection();

        /// <summary>
        /// 取得指定實體類型的 DbSet，用於查詢和操作該實體的資料
        /// </summary>
        /// <typeparam name="T">實體類型，必須是參考類型</typeparam>
        /// <returns>對應實體類型的 DbSet 物件</returns>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// 將目前上下文中的所有變更非同步儲存至資料庫
        /// </summary>
        /// <param name="cancellationToken">取消權杖，用於取消非同步操作</param>
        /// <returns>受影響的資料列數</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}