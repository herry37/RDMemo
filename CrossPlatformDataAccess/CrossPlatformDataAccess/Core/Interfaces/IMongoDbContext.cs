using MongoDB.Driver;

namespace CrossPlatformDataAccess.Core.Interfaces
{
    /// <summary>
    /// 定義 MongoDB 資料庫上下文的介面，提供存取 MongoDB 資料庫的基本功能
    /// </summary>
    public interface IMongoDbContext
    {
        /// <summary>
        /// 取得 MongoDB 資料庫實例，用於執行資料庫層級的操作
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        /// 取得指定名稱的 MongoDB 集合
        /// </summary>
        /// <typeparam name="T">集合中文件的類型</typeparam>
        /// <param name="name">MongoDB 集合的名稱</param>
        /// <returns>對應名稱的 MongoDB 集合物件</returns>
        IMongoCollection<T> GetCollection<T>(string name);
    }
}