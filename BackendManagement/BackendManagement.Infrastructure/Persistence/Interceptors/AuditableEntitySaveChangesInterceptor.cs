using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Domain.Common.Interfaces;

namespace BackendManagement.Infrastructure.Persistence.Interceptors;

/// <summary>
/// 可稽核實體儲存變更攔截器
/// </summary>
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly BackendManagement.Application.Common.Interfaces.IDateTime _dateTime;

    public AuditableEntitySaveChangesInterceptor(
        ICurrentUserService currentUserService,
        BackendManagement.Application.Common.Interfaces.IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BackendManagement.Domain.Common.Interfaces.IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _currentUserService.UserId;
                entry.Entity.Created = _dateTime.Now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedBy = _currentUserService.UserId;
                entry.Entity.LastModified = _dateTime.Now;
            }
        }
    }
} 