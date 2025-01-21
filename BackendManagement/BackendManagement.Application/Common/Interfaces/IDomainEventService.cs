using MediatR;

namespace BackendManagement.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task PublishAsync(Domain.Common.DomainEvent domainEvent);
} 