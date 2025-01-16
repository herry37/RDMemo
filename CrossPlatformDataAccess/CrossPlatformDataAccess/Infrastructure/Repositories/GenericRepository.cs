using CrossPlatformDataAccess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CrossPlatformDataAccess.Infrastructure.Repositories
{
    /// <summary>
    /// 通用儲存庫實作，提供基本的資料庫操作功能
    /// </summary>
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        // 資料庫上下文介面實例
        protected readonly IDbContext _context;
        // 實體的DbSet實例，用於直接操作資料表
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// 建構函式，初始化資料庫上下文和DbSet
        /// </summary>
        /// <param name="context">資料庫上下文介面</param>
        public GenericRepository(IDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// 根據ID非同步查詢單一實體
        /// </summary>
        /// <param name="id">實體ID</param>
        /// <returns>查詢到的實體</returns>
        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// 非同步獲取所有實體
        /// </summary>
        /// <returns>實體集合</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// 非同步新增實體
        /// </summary>
        /// <param name="entity">要新增的實體</param>
        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// 更新實體
        /// </summary>
        /// <param name="entity">要更新的實體</param>
        public virtual void Update(TEntity entity)
        {
            // 將實體附加到上下文並標記為已修改狀態
            _dbSet.Attach(entity);
            _context.Set<TEntity>().Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 刪除實體
        /// </summary>
        /// <param name="entity">要刪除的實體</param>
        public virtual void Delete(TEntity entity)
        {
            // 如果實體處於分離狀態，先將其附加到上下文
            if (_context.Set<TEntity>().Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// 根據條件查詢實體
        /// </summary>
        /// <param name="filter">查詢條件表達式</param>
        /// <returns>可查詢的實體集合</returns>
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null)
        {
            // 建立基礎查詢
            IQueryable<TEntity> query = _dbSet;
            // 如果有過濾條件，則應用條件
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query;
        }

        /// <summary>
        /// 執行原始SQL查詢
        /// </summary>
        /// <param name="sql">SQL查詢字串</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns>查詢結果實體列表</returns>
        public virtual async Task<List<TEntity>> ExecuteQueryAsync(string sql, params object[] parameters)
        {
            return await _dbSet.FromSqlRaw(sql, parameters).ToListAsync();
        }
    }
}