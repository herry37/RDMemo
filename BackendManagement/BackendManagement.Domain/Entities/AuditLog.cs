namespace BackendManagement.Domain.Entities;

public class AuditLog : EntityBase
{
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Changes { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? TenantId { get; set; }
} 