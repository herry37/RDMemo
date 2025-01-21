namespace BackendManagement.Domain.Common.Interfaces;

public interface ICurrentTenantService
{
    Guid? GetCurrentTenantId();
} 