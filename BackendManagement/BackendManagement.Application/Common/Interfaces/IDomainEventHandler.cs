using BackendManagement.Domain.Common;

namespace BackendManagement.Application.Common.Interfaces;

/// <summary>
/// 領域事件處理器介面
/// </summary>
/// <typeparam name="TEvent">領域事件類型</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// 處理領域事件
    /// </summary>
    /// <param name="domainEvent">領域事件</param>
    /// <param name="cancellationToken">取消權杖</param>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
} 