namespace BackendManagement.Domain.Events;

/// <summary>
/// 領域事件基礎類別
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// 事件發生時間
    /// </summary>
    public DateTime OccurredOn { get; }

    protected DomainEventBase()
    {
        OccurredOn = DateTime.UtcNow;
    }
} 