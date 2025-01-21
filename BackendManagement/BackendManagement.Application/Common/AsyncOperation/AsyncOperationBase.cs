namespace BackendManagement.Application.Common.AsyncOperation;
using BackendManagement.Domain.Constants;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Domain.Entities;
using BackendManagement.Domain.Common;

/// <summary>
/// 非同步操作基礎類別
/// </summary>
/// <typeparam name="TResult">操作結果類型</typeparam>
public abstract class AsyncOperationBase<TResult> : IAsyncOperation<TResult>
{
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogService _logService;
    private readonly string _operationName;
    private bool _isDisposed;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public Guid Id { get; } = Guid.NewGuid();
    
    private TResult? _result;
    private readonly TaskCompletionSource<TResult> _completionSource = new();

    /// <summary>
    /// 操作狀態
    /// </summary>
    public Interfaces.OperationStatus Status { get; private set; } = Interfaces.OperationStatus.Pending;

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="logService">日誌服務</param>
    /// <param name="operationName">操作名稱</param>
    /// <param name="maxConcurrency">最大並行數</param>
    protected AsyncOperationBase(
        ILogService logService,
        string operationName,
        int maxConcurrency = SystemConstants.Database.MaxConcurrency)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _operationName = string.IsNullOrEmpty(operationName) 
            ? throw new ArgumentException("操作名稱不可為空", nameof(operationName))
            : operationName;
        
        _semaphore = new SemaphoreSlim(maxConcurrency > 0 ? maxConcurrency : 1);
    }

    /// <summary>
    /// 執行非同步操作
    /// </summary>
    public async Task<TResult> ExecuteAsync(CancellationToken externalCancellation = default)
    {
        ThrowIfDisposed();

        // 合併外部和內部的取消權杖
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
            externalCancellation,
            _cancellationTokenSource.Token);

        try
        {
            Status = Interfaces.OperationStatus.Running;
            await _semaphore.WaitAsync(combinedCts.Token);

            _logService.Information($"開始執行操作: {_operationName}");
            var result = await ExecuteCoreAsync(combinedCts.Token);
            _result = result;
            _completionSource.TrySetResult(result);
            
            Status = Interfaces.OperationStatus.Completed;
            _logService.Information($"完成操作: {_operationName}");
            
            return result;
        }
        catch (OperationCanceledException)
        {
            Status = Interfaces.OperationStatus.Cancelled;
            _logService.Warning($"操作已取消: {_operationName}");
            throw;
        }
        catch (Exception ex)
        {
            Status = Interfaces.OperationStatus.Failed;
            _logService.Error(ex, $"執行操作失敗: {_operationName}");
            _completionSource.TrySetException(ex);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 取消操作
    /// </summary>
    public virtual async Task<bool> CancelAsync()
    {
        ThrowIfDisposed();
        try
        {
            _cancellationTokenSource.Cancel();
            return true;
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "取消操作失敗");
            return false;
        }
    }

    /// <summary>
    /// 實際執行的非同步操作
    /// </summary>
    protected abstract Task<TResult> ExecuteCoreAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 取得操作結果
    /// </summary>
    public virtual async Task<TResult> GetResultAsync()
    {
        return await _completionSource.Task;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _semaphore.Dispose();
                _cancellationTokenSource.Dispose();
            }
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    protected virtual async Task OnCancelledAsync()
    {
        await Task.CompletedTask;
    }

    protected virtual async Task OnStartAsync()
    {
        try
        {
            Status = Interfaces.OperationStatus.Running;
            await Task.CompletedTask; // 如果基礎類別不需要執行任何非同步操作，至少返回一個已完成的 Task
        }
        catch (Exception ex)
        {
            _logService.Error(ex, $"執行 {_operationName} 時發生錯誤");
            throw;
        }
    }
} 