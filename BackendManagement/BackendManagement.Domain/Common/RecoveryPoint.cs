namespace BackendManagement.Domain.Common;

public class RecoveryPoint
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string BackupPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
} 