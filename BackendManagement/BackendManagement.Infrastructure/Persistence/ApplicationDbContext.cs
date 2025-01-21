using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Domain.Common;
using BackendManagement.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendManagement.Infrastructure.Persistence;

/// <summary>
/// 應用程式資料庫上下文
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventService _domainEventService;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventService domainEventService,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _domainEventService = domainEventService;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchEvents();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEvents()
    {
        var entities = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        foreach (var entity in entities)
        {
            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();
            foreach (var @event in events)
            {
                await _domainEventService.PublishAsync(@event);
            }
        }
    }

    public DbSet<BackendManagement.Domain.Resilience.RecoveryPoint> RecoveryPoints => Set<BackendManagement.Domain.Resilience.RecoveryPoint>();
} 