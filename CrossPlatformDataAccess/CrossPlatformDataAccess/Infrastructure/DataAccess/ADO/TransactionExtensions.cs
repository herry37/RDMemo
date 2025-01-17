using System.Data;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.ADO
{
    public static class TransactionExtensions
    {
        /// <summary>
        /// 提交交易
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task CommitAsync(this IDbTransaction transaction)
        {
            // 檢查交易是否為空
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), $"'{nameof(transaction)}' 不能為 Null.");
            }
            try
            {
                // 實際的提交邏輯
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Commit 交易失敗", ex);
            }
        }

        /// <summary>
        /// 回滾交易
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task RollbackAsync(this IDbTransaction transaction)
        {
            // 檢查交易是否為空
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), $"'{nameof(transaction)}' 不能為 Null.");
            }
            try
            {
                // 實際的回滾邏輯           
                await transaction.RollbackAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Rollback 交易失敗", ex);
            }
        }
    }
}
