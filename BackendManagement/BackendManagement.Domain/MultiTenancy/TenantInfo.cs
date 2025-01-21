namespace BackendManagement.Domain.MultiTenancy;

public class TenantInfo : ITenantInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
} 