using MongoDB.Driver;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.MongoDB
{
    /// <summary>
    /// MongoDB 資料庫上下文介面
    /// </summary>
    public interface IMongoDbContext
    {
        /// <summary>
        /// 取得集合
        /// </summary>
        /// <typeparam name="T">實體類型</typeparam>
        /// <param name="name">集合名稱，如果為 null 則使用類型名稱</param>
        IMongoCollection<T> GetCollection<T>(string name = null) where T : class;

        /// <summary>
        /// 開始交易
        /// </summary>
        IClientSessionHandle StartSession();

        /// <summary>
        /// 開始交易（非同步）
        /// </summary>
        Task<IClientSessionHandle> StartSessionAsync();

        /// <summary>
        /// 取得資料庫
        /// </summary>
        IMongoDatabase Database { get; }
    }
}
