using CrossPlatformDataAccess.Common.Configuration;
using CrossPlatformDataAccess.Core.Interfaces;
using MongoDB.Driver;

namespace CrossPlatformDataAccess.Infrastructure.Context
{
    /// <summary>
    /// MongoDB 資料庫上下文實作
    /// </summary>
    public class MongoDbContext : IMongoDbContext
    {
        // MongoDB 資料庫實例
        private readonly IMongoDatabase _database;

        /// <summary>
        /// 建構子 - 初始化 MongoDB 連線
        /// </summary>
        /// <param name="config">MongoDB 連線設定</param>
        /// <exception cref="Exception">當 MongoDB 連線失敗時拋出例外</exception>
        public MongoDbContext(MongoDbConfig config)
        {
            try
            {
                // 建立 MongoDB 客戶端連線
                var client = new MongoClient(config.ConnectionString);
                // 取得指定的資料庫實例
                _database = client.GetDatabase(config.DatabaseName);
            }
            catch (Exception ex)
            {
                throw new Exception("MongoDB 連線失敗", ex);
            }
        }

        /// <summary>
        /// 取得 MongoDB 資料庫實例
        /// </summary>
        public IMongoDatabase Database => _database;

        /// <summary>
        /// 取得指定名稱的 MongoDB 集合
        /// </summary>
        /// <typeparam name="T">集合中文件的類型</typeparam>
        /// <param name="name">集合名稱</param>
        /// <returns>MongoDB 集合實例</returns>
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            // 從資料庫中取得指定名稱的集合
            return _database.GetCollection<T>(name);
        }
    }
}