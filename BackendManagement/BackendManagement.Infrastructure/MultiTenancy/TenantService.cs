namespace BackendManagement.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using BackendManagement.Application.Common.Interfaces;

/// <summary>
/// 租戶服務
/// </summary>
public class TenantService : ITenantService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenantService;

    public TenantService(
        ApplicationDbContext context,
        ICurrentTenantService currentTenantService)
    {
        _context = context;
        _currentTenantService = currentTenantService;
    }

    public async Task<Tenant?> GetTenantAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _currentTenantService.GetCurrentTenantId();
        if (tenantId == null) return null;
        
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant> CreateTenantAsync(
        string name,
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            Name = name,
            ConnectionString = connectionString,
            IsActive = true
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return tenant;
    }

    public async Task UpdateTenantAsync(
        Guid id,
        string name,
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync(new object[] { id }, cancellationToken);
        if (tenant == null)
            throw new KeyNotFoundException($"找不到租戶: {id}");

        tenant.Name = name;
        tenant.ConnectionString = connectionString;
        tenant.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTenantAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync(new object[] { id }, cancellationToken);
        if (tenant == null)
            throw new KeyNotFoundException($"找不到租戶: {id}");

        tenant.IsActive = false;
        tenant.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
} 