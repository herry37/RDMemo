namespace BackendManagement.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
} 