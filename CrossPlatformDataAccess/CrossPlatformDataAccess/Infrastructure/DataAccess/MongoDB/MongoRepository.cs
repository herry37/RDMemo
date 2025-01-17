using System.Linq.Expressions;
using MongoDB.Driver;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.MongoDB
{
    /// <summary>
    /// MongoDB 儲存庫實作
    /// </summary>
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _collection = _context.GetCollection<T>();
        }

        /// <summary>
        /// 取得集合
        /// </summary>
        public IMongoCollection<T> Collection => _collection;

        #region 同步操作

        /// <summary>
        /// 新增實體
        /// </summary>
        public void Add(T entity)
        {
            _collection.InsertOne(entity);
        }

        /// <summary>
        /// 批次新增實體
        /// </summary>
        public void AddMany(IEnumerable<T> entities)
        {
            _collection.InsertMany(entities);
        }

        /// <summary>
        /// 更新實體
        /// </summary>
        public bool Update(Expression<Func<T, bool>> filter, T entity)
        {
            var result = _collection.ReplaceOne(filter, entity, new ReplaceOptions { IsUpsert = false });
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// 刪除實體
        /// </summary>
        public bool Delete(Expression<Func<T, bool>> filter)
        {
            var result = _collection.DeleteOne(filter);
            return result.DeletedCount > 0;
        }

        /// <summary>
        /// 取得單一實體
        /// </summary>
        public T Get(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// 取得所有實體
        /// </summary>
        public IEnumerable<T> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }

        /// <summary>
        /// 依條件取得實體
        /// </summary>
        public IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).ToList();
        }

        /// <summary>
        /// 檢查是否存在
        /// </summary>
        public bool Exists(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).Any();
        }

        /// <summary>
        /// 計算數量
        /// </summary>
        public long Count(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null)
                return _collection.CountDocuments(_ => true);
            return _collection.CountDocuments(filter);
        }

        #endregion

        #region 非同步操作

        /// <summary>
        /// 新增實體
        /// </summary>
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
        }

        /// <summary>
        /// 批次新增實體
        /// </summary>
        public async Task AddManyAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _collection.InsertManyAsync(entities, new InsertManyOptions(), cancellationToken);
        }

        /// <summary>
        /// 更新實體
        /// </summary>
        public async Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, T entity, CancellationToken cancellationToken = default)
        {
            var result = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// 刪除實體
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }

        /// <summary>
        /// 取得單一實體
        /// </summary>
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 取得所有實體
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 依條件取得實體
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 檢查是否存在
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(filter).AnyAsync(cancellationToken);
        }

        /// <summary>
        /// 計算數量
        /// </summary>
        public async Task<long> CountAsync(Expression<Func<T, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            if (filter == null)
                return await _collection.CountDocumentsAsync(_ => true, null, cancellationToken);
            return await _collection.CountDocumentsAsync(filter, null, cancellationToken);
        }

        #endregion
    }
}
