using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Application.Common.Models.Requests;
using BackendManagement.Domain.Entities;

namespace BackendManagement.Infrastructure.Resilience;
using BackendManagement.Domain.Resilience;
using System.Security.Cryptography;
using BackendManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;

/// <summary>
/// 災難復原服務
/// </summary>
public class DisasterRecoveryService : IDisasterRecoveryService
{
    private readonly IBackupService _backupService;
    private readonly IDataSynchronizer _dataSynchronizer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DisasterRecoveryService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ILogService _logService;

    private static readonly Action<ILogger, string, Exception?> LogBackupCreated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(CreateRecoveryPointAsync)),
            "備份建立完成: {BackupPath}");

    private static readonly Action<ILogger, string, Exception?> LogBackupFailed =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(2, nameof(CreateRecoveryPointAsync)),
            "備份建立失敗: {ErrorMessage}");

    private static readonly Action<ILogger, string, string, Exception?> LogRestoreStarted =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(3, nameof(RestoreFromRecoveryPointAsync)),
            "開始從復原點還原: {Id}, {Description}");

    public DisasterRecoveryService(
        IBackupService backupService,
        IDataSynchronizer dataSynchronizer,
        IConfiguration configuration,
        ILogger<DisasterRecoveryService> logger,
        ApplicationDbContext context,
        ILogService logService)
    {
        _backupService = backupService;
        _dataSynchronizer = dataSynchronizer;
        _configuration = configuration;
        _logger = logger;
        _context = context;
        _logService = logService;
    }

    public async Task<bool> CreateRecoveryPointAsync(CreateRecoveryPointRequest request)
    {
        try
        {
            _logService.Information($"Creating recovery point: {request.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "Failed to create recovery point");
            return false;
        }
    }

    public async Task<bool> RestoreFromPointAsync(string pointId)
    {
        try
        {
            _logService.Information($"Restoring from point: {pointId}");
            return true;
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "Failed to restore from point");
            return false;
        }
    }

    public async Task<IEnumerable<Domain.Entities.RecoveryPoint>> GetRecoveryPointsAsync()
    {
        try
        {
            return await Task.FromResult(new List<Domain.Entities.RecoveryPoint>());
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "Failed to get recovery points");
            return Enumerable.Empty<Domain.Entities.RecoveryPoint>();
        }
    }

    public async Task<RecoveryPoint> CreateRecoveryPointAsync(string backupPath, CancellationToken cancellationToken = default)
    {
        var recoveryPoint = new RecoveryPoint
        {
            BackupPath = backupPath,
            Status = RecoveryPointStatus.Created,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _backupService.BackupDatabaseAsync(backupPath, cancellationToken);
            recoveryPoint.Checksum = await CalculateChecksumAsync(backupPath, cancellationToken);
            recoveryPoint.Status = RecoveryPointStatus.Available;
            
            await _context.RecoveryPoints.AddAsync(recoveryPoint, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            LogBackupCreated(_logger, backupPath, null);
            return recoveryPoint;
        }
        catch (Exception ex)
        {
            recoveryPoint.Status = RecoveryPointStatus.Failed;
            recoveryPoint.ErrorMessage = ex.Message;
            LogBackupFailed(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task RestoreFromRecoveryPointAsync(
        RecoveryPoint recoveryPoint,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await ValidateRecoveryPointAsync(recoveryPoint, cancellationToken);

            _logger.LogInformation(
                "開始從復原點還原: {id}, {description}",
                recoveryPoint.Id,
                recoveryPoint.Description);

            await _backupService.RestoreDatabaseAsync(recoveryPoint.BackupPath, cancellationToken);
            await _dataSynchronizer.SynchronizeDataAsync(cancellationToken);

            // 更新復原點狀態
            recoveryPoint.Status = RecoveryPointStatus.Recovered;
            recoveryPoint.RecoveredAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "復原點還原完成: {id}",
                recoveryPoint.Id);
        }
        catch (Exception ex)
        {
            recoveryPoint.Status = RecoveryPointStatus.Failed;
            recoveryPoint.ErrorMessage = ex.Message;
            _logger.LogError(ex, "從復原點還原失敗: {id}", recoveryPoint.Id);
            throw;
        }
    }

    public async Task ValidateRecoveryPointAsync(
        RecoveryPoint recoveryPoint,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (recoveryPoint == null)
            {
                _logger.LogWarning("復原點不存在");
                throw new ArgumentNullException(nameof(recoveryPoint));
            }

            // 檢查檔案是否存在
            if (!File.Exists(recoveryPoint.BackupPath))
            {
                recoveryPoint.Status = RecoveryPointStatus.Invalid;
                recoveryPoint.ErrorMessage = "備份檔案不存在";
                await _context.SaveChangesAsync(cancellationToken);
                throw new FileNotFoundException("備份檔案不存在", recoveryPoint.BackupPath);
            }

            // 驗證檔案校驗和
            var currentChecksum = await CalculateChecksumAsync(recoveryPoint.BackupPath, cancellationToken);
            if (currentChecksum != recoveryPoint.Checksum)
            {
                _logger.LogWarning("備份檔案校驗和不符");
                recoveryPoint.Status = RecoveryPointStatus.Invalid;
                recoveryPoint.ErrorMessage = "備份檔案校驗和不符";
                await _context.SaveChangesAsync(cancellationToken);
                throw new InvalidOperationException("備份檔案校驗和不符");
            }

            recoveryPoint.Status = RecoveryPointStatus.Validated;
            recoveryPoint.ValidatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            recoveryPoint.Status = RecoveryPointStatus.Failed;
            recoveryPoint.ErrorMessage = ex.Message;
            _logger.LogError(ex, "復原點驗證失敗: {id}", recoveryPoint.Id);
            throw;
        }
    }

    private async Task<string> CalculateChecksumAsync(string filePath, CancellationToken cancellationToken)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }
} 