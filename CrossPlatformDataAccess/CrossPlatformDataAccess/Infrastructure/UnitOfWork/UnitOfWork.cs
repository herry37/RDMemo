using CrossPlatformDataAccess.Core.Interfaces;
using CrossPlatformDataAccess.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace CrossPlatformDataAccess.Infrastructure.UnitOfWork
{
    /// <summary>
    ///  工作單元實作類別，負責管理資料庫交易與儲存庫的存取
    /// </summary>
    /// <remarks>
    ///  此類別負責：
    ///  1. 管理資料庫交易的生命週期
    ///  2. 提供儲存庫的存取介面
    ///  3. 確保資源的正確釋放
    ///  4. 實作工作單元模式(Unit of Work Pattern)
    /// </remarks>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        //  資料庫上下文介面實例，用於存取資料庫
        private readonly IDbContext _context;
        //  資料庫交易介面實例，用於管理交易生命週期
        private IDbContextTransaction _transaction;
        //  儲存庫快取字典，用於儲存已建立的儲存庫實例，避免重複建立
        private readonly ConcurrentDictionary<string, object> _repositories;
        //  標記此實例是否已被釋放
        private bool _disposed;

        /// <summary>
        ///  建構子，初始化工作單元
        /// </summary>
        /// <param name="context">資料庫上下文實例</param>
        /// <exception cref="ArgumentNullException">當context參數為null時擲出</exception>
        /// <remarks>
        ///  初始化資料庫上下文與儲存庫快取字典
        /// </remarks>
        public UnitOfWork(IDbContext context, IDbContextTransaction transaction)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new ConcurrentDictionary<string, object>();
            _transaction = transaction;
        }

        /// <summary>
        ///  取得指定實體類型的儲存庫實例
        /// </summary>
        /// <typeparam name="TEntity">實體類型</typeparam>
        /// <returns>對應實體類型的儲存庫實例</returns>
        /// <remarks>
        ///  如果儲存庫不存在則建立新的實例並加入快取
        ///  使用ConcurrentDictionary確保執行緒安全
        /// </remarks>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;

            return (IRepository<TEntity>)_repositories.GetOrAdd(type,
                _ => new GenericRepository<TEntity>(_context));
        }

        /// <summary>
        ///  儲存所有變更至資料庫
        /// </summary>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>受影響的資料列數</returns>
        /// <exception cref="ObjectDisposedException">當物件已被釋放時擲出</exception>
        /// <remarks>
        ///  呼叫資料庫上下文的SaveChanges方法，將所有變更寫入資料庫
        /// </remarks>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
            {
                ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
            }
            return await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///  開始一個新的資料庫交易
        /// </summary>
        /// <exception cref="ObjectDisposedException">當物件已被釋放時擲出</exception>
        /// <exception cref="InvalidOperationException">當已有進行中的交易時擲出</exception>
        /// <remarks>
        ///  建立並儲存新的資料庫交易實例
        /// </remarks>
        public async Task BeginTransactionAsync()
        {
            if (_disposed)
            {
                ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
            }
            if (_transaction != null)
            {
                throw new InvalidOperationException("交易已經開始");
            }

            _transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///  提交目前的資料庫交易
        /// </summary>
        /// <exception cref="ObjectDisposedException">當物件已被釋放時擲出</exception>
        /// <exception cref="InvalidOperationException">當沒有進行中的交易時擲出</exception>
        /// <remarks>
        ///  提交交易並釋放交易資源
        /// </remarks>
        public async Task CommitTransactionAsync()
        {
            if (_disposed)
            {
                ObjectDisposedException.ThrowIf(_disposed, (nameof(UnitOfWork)));
            }
            if (_transaction == null)
            {
                throw new InvalidOperationException("沒有進行中的交易可提交");
            }

            try
            {
                await _transaction.CommitAsync().ConfigureAwait(false);
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync().ConfigureAwait(false);
                    _transaction = null!;
                }
            }
        }

        /// <summary>
        ///  回滾目前的資料庫交易
        /// </summary>
        /// <exception cref="ObjectDisposedException">當物件已被釋放時擲出</exception>
        /// <exception cref="InvalidOperationException">當沒有進行中的交易時擲出</exception>
        /// <remarks>
        ///  回滾交易並釋放交易資源
        /// </remarks>
        public async Task RollbackTransactionAsync()
        {
            if (_disposed)
            {
                ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
            }
            if (_transaction == null)
            {
                throw new InvalidOperationException(" ");
            }

            try
            {
                await _transaction.RollbackAsync().ConfigureAwait(false);
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync().ConfigureAwait(false);
                    _transaction = null!;
                }
            }
        }

        /// <summary>
        ///  釋放所有使用的資源
        /// </summary>
        /// <remarks>
        ///  實作IDisposable介面，確保資源正確釋放
        /// </remarks>
        public void Dispose()
        {
            // 釋放所有資源
            Dispose(true);
            // 指示垃圾回收器不再呼叫最終化方法
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  釋放資源的受保護方法
        /// </summary>
        /// <param name="disposing">是否正在釋放受控資源</param>
        /// <remarks>
        ///  釋放交易、資料庫上下文與儲存庫資源
        ///  此方法可被覆寫以自訂釋放邏輯
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // 釋放交易資源
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null!;
                }
                // 釋放資料庫上下文資源
                _context?.Dispose();
                // 清除儲存庫快取
                _repositories.Clear();
            }
            _disposed = true;
        }

        /// <summary>
        ///  解構子
        /// </summary>
        /// <remarks>
        ///  當物件被回收時呼叫Dispose方法
        ///  作為最後的防線確保資源被釋放
        /// </remarks>
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}