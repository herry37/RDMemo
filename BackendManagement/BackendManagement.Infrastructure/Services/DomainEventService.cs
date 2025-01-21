using MediatR;
using Microsoft.Extensions.Logging;
using BackendManagement.Domain.Common;
using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Application.Common.Models;

namespace BackendManagement.Infrastructure.Services;

/// <summary>
/// 領域事件服務
/// </summary>
public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _mediator;

    public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task PublishAsync(DomainEvent domainEvent)
    {
        _logger.LogInformation("Publishing domain event: {EventName}", domainEvent.GetType().Name);
        await _mediator.Publish(domainEvent);
    }
} 