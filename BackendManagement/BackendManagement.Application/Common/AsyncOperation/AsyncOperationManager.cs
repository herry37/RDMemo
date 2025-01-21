using BackendManagement.Application.Common.Interfaces;
using System.Collections.Concurrent;

namespace BackendManagement.Application.Common.AsyncOperation;

/// <summary>
/// 非同步操作管理器
/// </summary>
public class AsyncOperationManager
{
    private readonly ConcurrentDictionary<Guid, IAsyncOperation<object>> _operations = new();
    private readonly ILogService _logService;

    /// <summary>
    /// 建構函式
    /// </summary>
    public AsyncOperationManager(ILogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// 註冊非同步操作
    /// </summary>
    public void RegisterOperation<TOperation, TResult>(string operationId)
        where TOperation : IAsyncOperation<TResult>
    {
        if (_operations.TryAdd(Guid.Parse(operationId),
            (IAsyncOperation<object>)Activator.CreateInstance(typeof(TOperation), _logService)!))
        {
            _logService.Information($"已註冊操作: {operationId}");
        }
    }

    /// <summary>
    /// 取得操作狀態
    /// </summary>
    public Interfaces.OperationStatus GetOperationStatus(string operationId)
    {
        if (_operations.TryGetValue(Guid.Parse(operationId), out var operation))
        {
            return operation.Status;
        }
        return Interfaces.OperationStatus.NotFound;
    }

    /// <summary>
    /// 取消操作
    /// </summary>
    public async Task CancelOperationAsync(string operationId)
    {
        if (_operations.TryGetValue(Guid.Parse(operationId), out var operation))
        {
            await operation.CancelAsync();
            _logService.Warning($"已取消操作: {operationId}");
        }
    }
}