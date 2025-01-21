namespace BackendManagement.Application.Common.Interfaces;

public interface ITenantService : IApplicationService
{
    Task<Domain.MultiTenancy.Tenant?> GetTenantAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.MultiTenancy.Tenant>> GetAllTenantsAsync(CancellationToken cancellationToken = default);
    Task<Domain.MultiTenancy.Tenant> CreateTenantAsync(string name, string connectionString, CancellationToken cancellationToken = default);
    Task UpdateTenantAsync(Guid id, string name, string connectionString, CancellationToken cancellationToken = default);
    Task DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default);
} 