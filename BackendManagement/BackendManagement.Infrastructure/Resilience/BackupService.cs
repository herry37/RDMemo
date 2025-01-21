using BackendManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BackendManagement.Infrastructure.Resilience;

/// <summary>
/// 備份服務
/// </summary>
public class BackupService : IBackupService
{
    private readonly ILogger<BackupService> _logger;
    private readonly ApplicationDbContext _context;

    public BackupService(
        ILogger<BackupService> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<string> BackupDatabaseAsync(string backupPath, CancellationToken cancellationToken = default)
    {
        try
        {
            // 確保備份目錄存在
            var directory = Path.GetDirectoryName(backupPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 執行資料庫備份
            await using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync(cancellationToken);
            
            // 這裡需要根據實際使用的資料庫類型實作備份邏輯
            // 以下為示例
            using var command = connection.CreateCommand();
            command.CommandText = $"BACKUP DATABASE {_context.Database.GetDbConnection().Database} TO DISK = '{backupPath}'";
            await command.ExecuteNonQueryAsync(cancellationToken);

            return backupPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "資料庫備份失敗: {Path}", backupPath);
            throw;
        }
    }

    public async Task RestoreDatabaseAsync(string backupPath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(backupPath))
            {
                throw new FileNotFoundException("備份檔案不存在", backupPath);
            }

            // 執行資料庫還原
            await using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync(cancellationToken);
            
            // 這裡需要根據實際使用的資料庫類型實作還原邏輯
            // 以下為示例
            using var command = connection.CreateCommand();
            command.CommandText = $"RESTORE DATABASE {_context.Database.GetDbConnection().Database} FROM DISK = '{backupPath}'";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "資料庫還原失敗: {Path}", backupPath);
            throw;
        }
    }
} 