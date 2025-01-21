using BackendManagement.Application.Common.Interfaces;

namespace BackendManagement.Application.Common.DataProcessors;
using BackendManagement.Domain.Constants;

/// <summary>
/// 資料處理器基礎類別
/// </summary>
public abstract class DataProcessorBase<TInput, TOutput> : IDataProcessor<TInput, TOutput>
{
    protected readonly ILogService _logService;

    protected DataProcessorBase(ILogService logService)
    {
        _logService = logService;
    }

    public virtual TOutput Process(TInput input)
    {
        try
        {
            _logService.Information($"開始處理資料: {typeof(TInput).Name} -> {typeof(TOutput).Name}");
            var result = ProcessCore(input);
            _logService.Information("資料處理完成");
            return result;
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "資料處理失敗");
            throw;
        }
    }

    public virtual async Task<TOutput> ProcessAsync(TInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            _logService.Information($"開始非同步處理資料: {typeof(TInput).Name} -> {typeof(TOutput).Name}");
            var result = await ProcessCoreAsync(input, cancellationToken);
            _logService.Information("資料處理完成");
            return result;
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "資料處理失敗");
            throw;
        }
    }

    public virtual async Task<IEnumerable<TOutput>> ProcessBatchAsync(
        IEnumerable<TInput> inputs,
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var results = new List<TOutput>();
        var batch = new List<TInput>();
        
        foreach (var input in inputs)
        {
            batch.Add(input);
            if (batch.Count >= batchSize)
            {
                var batchResults = await ProcessBatchCoreAsync(batch, cancellationToken);
                results.AddRange(batchResults);
                batch.Clear();
            }
        }

        if (batch.Any())
        {
            var remainingResults = await ProcessBatchCoreAsync(batch, cancellationToken);
            results.AddRange(remainingResults);
        }

        return results;
    }

    protected abstract TOutput ProcessCore(TInput input);
    protected abstract Task<TOutput> ProcessCoreAsync(TInput input, CancellationToken cancellationToken);
    protected abstract Task<IEnumerable<TOutput>> ProcessBatchCoreAsync(
        IEnumerable<TInput> inputs,
        CancellationToken cancellationToken);
} 