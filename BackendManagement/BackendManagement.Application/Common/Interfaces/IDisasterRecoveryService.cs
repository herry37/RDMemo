using BackendManagement.Application.Common.Models.Requests;
using BackendManagement.Domain.Entities;

namespace BackendManagement.Application.Common.Interfaces;

public interface IDisasterRecoveryService
{
    Task<bool> CreateRecoveryPointAsync(CreateRecoveryPointRequest request);
    Task<bool> RestoreFromPointAsync(string pointId);
    Task<IEnumerable<BackendManagement.Domain.Entities.RecoveryPoint>> GetRecoveryPointsAsync();
} 