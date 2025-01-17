using CrossPlatformDataAccess.Common.Configuration;
using MongoDB.Driver;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.MongoDB
{
    /// <summary>
    /// MongoDB 資料庫上下文實作
    /// </summary>
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _client;

        public MongoDbContext(MongoDbConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (!config.Validate())
                throw new ArgumentException("MongoDB configuration is invalid", nameof(config));

            _client = new MongoClient(config.GetConnectionString());
            _database = _client.GetDatabase(config.Database);
        }

        /// <summary>
        /// 取得資料庫
        /// </summary>
        public IMongoDatabase Database => _database;

        /// <summary>
        /// 取得集合
        /// </summary>
        public IMongoCollection<T> GetCollection<T>(string name = null) where T : class
        {
            return _database.GetCollection<T>(name ?? typeof(T).Name);
        }

        /// <summary>
        /// 開始交易
        /// </summary>
        public IClientSessionHandle StartSession()
        {
            return _client.StartSession();
        }

        /// <summary>
        /// 開始交易（非同步）
        /// </summary>
        public async Task<IClientSessionHandle> StartSessionAsync()
        {
            return await _client.StartSessionAsync();
        }
    }
}
