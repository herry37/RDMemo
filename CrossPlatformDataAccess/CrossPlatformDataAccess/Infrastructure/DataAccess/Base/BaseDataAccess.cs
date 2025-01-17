using System;
using System.Threading.Tasks;
using System.Threading;
using CrossPlatformDataAccess.Core.Logging;
using CrossPlatformDataAccess.Core.Transaction;
using CrossPlatformDataAccess.Core.DataAccess;

namespace CrossPlatformDataAccess.Infrastructure.DataAccess.Base
{
    /// <summary>
    /// 資料存取基礎類別
    /// 提供交易管理和日誌記錄功能
    /// </summary>
    public abstract class BaseDataAccess
    {
        protected readonly ILogger _logger;
        protected readonly IDataAccessStrategy _strategy;

        protected BaseDataAccess(IDataAccessStrategy strategy, ILogger logger)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 在交易中執行資料庫操作
        /// </summary>
        protected async Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<Task<TResult>> operation,
            string operationName,
            CancellationToken cancellationToken = default)
        {
            using var transaction = await _strategy.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogInformation($"開始執行資料庫操作: {operationName}");
                var result = await operation();
                await transaction.CommitAsync();
                _logger.LogInformation($"成功完成資料庫操作: {operationName}");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"資料庫操作失敗: {operationName}", ex);
                throw;
            }
        }

        /// <summary>
        /// 在交易中執行無回傳值的資料庫操作
        /// </summary>
        protected async Task ExecuteInTransactionAsync(
            Func<Task> operation,
            string operationName,
            CancellationToken cancellationToken = default)
        {
            using var transaction = await _strategy.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogInformation($"開始執行資料庫操作: {operationName}");
                await operation();
                await transaction.CommitAsync();
                _logger.LogInformation($"成功完成資料庫操作: {operationName}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"資料庫操作失敗: {operationName}", ex);
                throw;
            }
        }

        /// <summary>
        /// 執行查詢操作（不需要交易）
        /// </summary>
        protected async Task<TResult> ExecuteQueryAsync<TResult>(
            Func<Task<TResult>> query,
            string queryName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"開始執行查詢: {queryName}");
                var result = await query();
                _logger.LogInformation($"成功完成查詢: {queryName}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"查詢失敗: {queryName}", ex);
                throw;
            }
        }
    }
}
