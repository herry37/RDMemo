using System;
using System.Data;
using System.Threading.Tasks;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.ADO
{
    public static class TransactionExtensions
    {
        public static async Task CommitAsync(this IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            // 實際的提交邏輯
            await Task.Run(() => transaction.Commit());
        }

        public static async Task RollbackAsync(this IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            // 實際的回滾邏輯
            await Task.Run(() => transaction.Rollback());
        }
    }
}
