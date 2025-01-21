namespace BackendManagement.Application.Common.Interfaces;

/// <summary>
/// 資料處理介面
/// </summary>
/// <typeparam name="TInput">輸入類型</typeparam>
/// <typeparam name="TOutput">輸出類型</typeparam>
public interface IDataProcessor<TInput, TOutput>
{
    /// <summary>
    /// 同步處理資料
    /// </summary>
    TOutput Process(TInput input);

    /// <summary>
    /// 非同步處理資料
    /// </summary>
    Task<TOutput> ProcessAsync(TInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次處理資料
    /// </summary>
    Task<IEnumerable<TOutput>> ProcessBatchAsync(
        IEnumerable<TInput> inputs,
        int batchSize = 100,
        CancellationToken cancellationToken = default);
} 