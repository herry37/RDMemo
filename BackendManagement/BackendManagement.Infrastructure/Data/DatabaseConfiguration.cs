using Microsoft.EntityFrameworkCore;
using EFCoreSecondLevelCacheInterceptor;

namespace BackendManagement.Infrastructure.Data;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            x => x.EnableRetryOnFailure()
        );
    }
} 