using CrossPlatformDataAccess.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CrossPlatformDataAccess.Infrastructure.Repositories
{
    /// <summary>
    /// MongoDB 儲存庫實作，提供對 MongoDB 文件的基本 CRUD 操作功能
    /// </summary>
    /// <typeparam name="TDocument">文件類型，必須是參考類型</typeparam>
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : class
    {
        // MongoDB 集合實例，用於直接操作文件
        private readonly IMongoCollection<TDocument> _collection;

        /// <summary>
        /// 建構函式，初始化 MongoDB 集合
        /// </summary>
        /// <param name="context">MongoDB 資料庫上下文</param>
        /// <param name="collectionName">集合名稱</param>
        public MongoRepository(IMongoDbContext context, string collectionName)
        {
            // 從上下文取得指定名稱的集合
            _collection = context.GetCollection<TDocument>(collectionName);
        }

        /// <summary>
        /// 非同步取得所有文件
        /// </summary>
        /// <returns>所有文件的集合</returns>
        /// <remarks>使用空的過濾條件來取得所有文件</remarks>
        public async Task<IEnumerable<TDocument>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// 非同步依據 ID 取得單一文件
        /// </summary>
        /// <param name="id">文件 ID</param>
        /// <returns>符合 ID 的文件，若不存在則返回 null</returns>
        /// <remarks>將字串 ID 轉換為 ObjectId 並建立過濾條件</remarks>
        public async Task<TDocument> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<TDocument>.Filter.Eq("_id", objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 非同步新增文件
        /// </summary>
        /// <param name="document">要新增的文件</param>
        /// <remarks>處理可能發生的 MongoDB 寫入異常</remarks>
        public async Task CreateAsync(TDocument document)
        {
            try
            {
                await _collection.InsertOneAsync(document);
            }
            catch (MongoWriteException ex)
            {
                throw new Exception("MongoDB 寫入錯誤", ex);
            }
        }

        /// <summary>
        /// 非同步更新文件
        /// </summary>
        /// <param name="id">要更新的文件 ID</param>
        /// <param name="document">包含更新內容的文件</param>
        /// <remarks>使用 ReplaceOne 來完整替換現有文件</remarks>
        public async Task UpdateAsync(string id, TDocument document)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<TDocument>.Filter.Eq("_id", objectId);
            await _collection.ReplaceOneAsync(filter, document);
        }

        /// <summary>
        /// 非同步刪除文件
        /// </summary>
        /// <param name="id">要刪除的文件 ID</param>
        /// <remarks>使用 DeleteOne 來刪除單一符合條件的文件</remarks>
        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<TDocument>.Filter.Eq("_id", objectId);
            await _collection.DeleteOneAsync(filter);
        }

        /// <summary>
        /// 非同步依據條件尋找文件
        /// </summary>
        /// <param name="filterExpression">過濾條件表達式</param>
        /// <returns>符合條件的文件集合</returns>
        /// <remarks>支援使用 LINQ 表達式進行複雜查詢</remarks>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return await _collection.Find(filterExpression).ToListAsync();
        }
    }
}