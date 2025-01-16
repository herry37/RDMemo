namespace CrossPlatformDataAccess.Core.Interfaces
{
    /// <summary>
    /// 定義工作單元的介面，提供資料庫交易和儲存庫管理的功能
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 取得指定實體類型的儲存庫實例
        /// </summary>
        /// <typeparam name="TEntity">實體類型，必須是參考類型</typeparam>
        /// <returns>對應實體類型的儲存庫物件</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// 非同步儲存所有變更至資料庫
        /// </summary>
        /// <param name="cancellationToken">取消權杖，用於取消非同步操作</param>
        /// <returns>受影響的資料列數</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 非同步開始一個新的資料庫交易
        /// </summary>
        /// <returns>表示非同步操作的工作</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// 非同步提交目前的資料庫交易
        /// </summary>
        /// <returns>表示非同步操作的工作</returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// 非同步回滾目前的資料庫交易
        /// </summary>
        /// <returns>表示非同步操作的工作</returns>
        Task RollbackTransactionAsync();
    }
}
