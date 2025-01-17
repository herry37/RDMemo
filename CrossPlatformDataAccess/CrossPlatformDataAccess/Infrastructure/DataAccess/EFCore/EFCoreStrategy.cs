using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CrossPlatformDataAccess.Core.DataAccess;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.EFCore
{
    /// <summary>
    /// EF Core 實作的資料存取策略
    /// </summary>
    public class EFCoreStrategy : IDataAccessStrategy
    {
        private readonly DbContext _context;

        public EFCoreStrategy(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class
        {
            return await _context.Set<T>().Where(predicate).ToListAsync(cancellationToken);
        }

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new EFCoreQueryBuilder<T>(_context);
        }

        public async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new EFCoreTransactionScope(transaction);
        }
    }
}
