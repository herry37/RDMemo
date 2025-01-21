namespace BackendManagement.Application.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;

public class RecoveryPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string BackupPath { get; set; } = string.Empty;
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) 
            && !string.IsNullOrEmpty(Description)
            && !string.IsNullOrEmpty(BackupPath)
            && File.Exists(BackupPath);
    }
} 