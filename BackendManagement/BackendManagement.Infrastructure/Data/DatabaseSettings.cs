namespace BackendManagement.Infrastructure.Data;

public class DatabaseSettings
{
    public int CommandTimeout { get; set; } = 30;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxBatchSize { get; set; } = 100;
    public bool EnableSecondLevelCache { get; set; } = true;
} 