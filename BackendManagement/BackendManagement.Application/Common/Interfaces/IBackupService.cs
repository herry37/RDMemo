namespace BackendManagement.Application.Common.Interfaces;

public interface IBackupService
{
    Task<string> BackupDatabaseAsync(string backupPath, CancellationToken cancellationToken = default);
    Task RestoreDatabaseAsync(string backupPath, CancellationToken cancellationToken = default);
} 