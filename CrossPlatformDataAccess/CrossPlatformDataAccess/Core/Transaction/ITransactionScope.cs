using System;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Core.Transaction
{
    /// <summary>
    /// 交易範圍介面
    /// </summary>
    public interface ITransactionScope : IAsyncDisposable
    {
        /// <summary>
        /// 提交交易
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// 回滾交易
        /// </summary>
        Task RollbackAsync();
    }
}
