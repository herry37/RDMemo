namespace BackendManagement.Application.Common.Interfaces;

public interface IDataSynchronizer
{
    Task SynchronizeDataAsync(CancellationToken cancellationToken = default);
} 