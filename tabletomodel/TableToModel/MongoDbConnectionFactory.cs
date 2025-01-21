using System;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TableToModel
{
    /// <summary>
    /// MongoDB 連線工廠
    /// </summary>
    public class MongoDbConnectionFactory : IDatabaseConnectionFactory
    {
        private IMongoDatabase? _database;
        private readonly IMongoClient _client;

        public MongoDbConnectionFactory()
        {
            _client = null!;
        }

        /// <summary>
        /// 創建 MongoDB 連線
        /// </summary>
        public IDbConnection CreateConnection(string connectionString)
        {
            throw new NotSupportedException("MongoDB 不支援 IDbConnection 介面，請使用 GetMongoDatabase() 方法");
        }

        /// <summary>
        /// 取得 MongoDB 資料庫實例
        /// </summary>
        public IMongoDatabase GetMongoDatabase(string connectionString)
        {
            if (_database == null)
            {
                var client = new MongoClient(connectionString);
                var url = new MongoUrl(connectionString);
                _database = client.GetDatabase(url.DatabaseName);
            }
            return _database;
        }

        /// <summary>
        /// 生成 MongoDB 連線字串
        /// </summary>
        public string GetConnectionString(string server, string userId, string password, string database)
        {
            // 建構 MongoDB 連線字串
            return $"mongodb://{Uri.EscapeDataString(userId)}:{Uri.EscapeDataString(password)}@{server}/{database}";
        }

        /// <summary>
        /// 取得集合的結構資訊
        /// </summary>
        public async Task<BsonDocument> GetCollectionSchema(string collectionName)
        {
            if (_database == null)
                throw new InvalidOperationException("請先呼叫 GetMongoDatabase() 方法");

            var collection = _database.GetCollection<BsonDocument>(collectionName);

            // 使用 aggregation 來分析集合結構
            var pipeline = new[]
            {
                new BsonDocument("$sample", new BsonDocument("size", 100)),
                new BsonDocument("$project",
                    new BsonDocument("_id", 0)
                        .Add("fields",
                            new BsonDocument("$objectToArray", "$$ROOT"))),
                new BsonDocument("$unwind", "$fields"),
                new BsonDocument("$group",
                    new BsonDocument("_id", "$fields.k")
                        .Add("types",
                            new BsonDocument("$addToSet",
                                new BsonDocument("$type", "$fields.v"))))
            };

            var result = await collection.AggregateAsync<BsonDocument>(pipeline);
            return await result.FirstOrDefaultAsync();
        }
    }
}
