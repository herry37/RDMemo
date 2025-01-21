namespace BackendManagement.Domain.MultiTenancy;

public class Tenant : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public new DateTime? LastModifiedAt { get; set; }
} 