using System.Linq.Expressions;
using CrossPlatformDataAccess.Core.DataAccess;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.MongoDB
{
    /// <summary>
    /// MongoDB 資料庫提供者
    /// </summary>
    public class MongoDbProvider : IDataAccessStrategy
    {
        private readonly IMongoDatabase _database;
        private readonly IClientSessionHandle _session;

        public MongoDbProvider(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _session = client.StartSession();
        }

        public IDbProvider Provider => throw new NotImplementedException();

        #region CRUD Operations

        public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.InsertOneAsync(entity, null, cancellationToken);
            return entity;
        }

        public async Task<bool> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", GetEntityId(entity));
            var result = await collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", GetEntityId(entity));
            var result = await collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<T> GetByIdAsync<T>(object id, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            return await collection.Find(new BsonDocument()).ToListAsync(cancellationToken);
        }

        #endregion

        #region Query Operations

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new MongoDbQueryBuilder<T>(_database.GetCollection<T>(typeof(T).Name));
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string filter, object parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var bsonFilter = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(filter);
            return await collection.Find(bsonFilter).ToListAsync(cancellationToken);
        }

        public int Execute(string command, object parameters = null)
        {
            var result = _database.RunCommand<BsonDocument>(command);
            return result["ok"].AsInt32;
        }

        public async Task<int> ExecuteAsync(string command, object parameters = null, CancellationToken cancellationToken = default)
        {
            var result = await _database.RunCommandAsync<BsonDocument>(command, null, cancellationToken);
            return result["ok"].AsInt32;
        }

        #endregion

        #region Transaction Management

        public IDbTransaction BeginTransaction()
        {
            _session.StartTransaction();
            return new MongoDbTransaction(_session);
        }

        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _session.StartTransactionAsync(null, cancellationToken);
            return new MongoDbTransaction(_session);
        }

        public T ExecuteInTransaction<T>(Func<T> operation)
        {
            using var transaction = BeginTransaction();
            try
            {
                var result = operation();
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            using var transaction = await BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await operation();
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        #endregion

        #region Bulk Operations

        public async void BulkInsert<T>(IEnumerable<T> entities) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.InsertManyAsync(entities);
        }

        public async Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            await collection.InsertManyAsync(entities, new InsertManyOptions(), cancellationToken);
        }

        public void BulkUpdate<T>(IEnumerable<T> entities) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var bulkOps = entities.Select(e =>
            {
                var filter = Builders<T>.Filter.Eq("_id", GetEntityId(e));
                return new ReplaceOneModel<T>(filter, e);
            });
            collection.BulkWrite(bulkOps);
        }

        public async Task BulkUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var bulkOps = entities.Select(e =>
            {
                var filter = Builders<T>.Filter.Eq("_id", GetEntityId(e));
                return new ReplaceOneModel<T>(filter, e);
            });
            await collection.BulkWriteAsync(bulkOps, new BulkWriteOptions(), cancellationToken);
        }

        public void BulkDelete<T>(IEnumerable<T> entities) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var bulkOps = entities.Select(e =>
            {
                var filter = Builders<T>.Filter.Eq("_id", GetEntityId(e));
                return new DeleteOneModel<T>(filter);
            });
            collection.BulkWrite(bulkOps);
        }

        public async Task BulkDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var bulkOps = entities.Select(e =>
            {
                var filter = Builders<T>.Filter.Eq("_id", GetEntityId(e));
                return new DeleteOneModel<T>(filter);
            });
            await collection.BulkWriteAsync(bulkOps, new BulkWriteOptions(), cancellationToken);
        }

        #endregion

        #region IDataAccessStrategy

        public T Add<T>(T entity) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            collection.InsertOne(entity);
            return entity;
        }

        public bool Update<T>(T entity) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", GetEntityId(entity));
            var result = collection.ReplaceOne(filter, entity, new ReplaceOptions { IsUpsert = false });
            return result.ModifiedCount > 0;
        }

        public bool Delete<T>(T entity) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", GetEntityId(entity));
            var result = collection.DeleteOne(filter);
            return result.DeletedCount > 0;
        }

        public T GetById<T>(object id) where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return collection.Find(filter).FirstOrDefault();
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            return collection.Find(new BsonDocument()).ToList();
        }

        public IEnumerable<T> Query<T>(string sql, object? parameters = null) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        public int Execute(string sql, object? parameters = null)
        {
            throw new NotImplementedException();
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, object? parameters = null) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Methods

        private object GetEntityId<T>(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("_id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} must have an Id or _id property");
            return idProperty.GetValue(entity);
        }

        public void Dispose()
        {
            _session?.Dispose();
        }

        System.Data.IDbTransaction IDataAccessStrategy.BeginTransaction()
        {
            throw new NotImplementedException();
        }

        Task<System.Data.IDbTransaction> IDataAccessStrategy.BeginTransactionAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// MongoDB 交易實作
    /// </summary>
    public class MongoDbTransaction : IDbTransaction
    {
        private readonly IClientSessionHandle _session;
        private bool _disposed;

        public MongoDbTransaction(IClientSessionHandle session)
        {
            _session = session;
        }

        public void Commit()
        {
            _session.CommitTransaction();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _session.CommitTransactionAsync(cancellationToken);
        }

        public void Rollback()
        {
            _session.AbortTransaction();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _session.AbortTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _session.Dispose();
                _disposed = true;
            }
        }
    }
}

