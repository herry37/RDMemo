namespace BackendManagement.Infrastructure.Persistence;

/// <summary>
/// 資料庫類型列舉
/// </summary>
public enum DatabaseType
{
    MsSql,
    MySql,
    PostgreSql,
    Sqlite
}

/// <summary>
/// 資料庫工廠類別
/// </summary>
public class DatabaseFactory
{
    /// <summary>
    /// 建立資料庫上下文
    /// </summary>
    public static DbContextOptions<ApplicationDbContext> CreateDbContext(
        DatabaseType databaseType,
        string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        switch (databaseType)
        {
            case DatabaseType.MsSql:
                optionsBuilder.UseSqlServer(connectionString);
                break;
            case DatabaseType.MySql:
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                break;
            case DatabaseType.PostgreSql:
                optionsBuilder.UseNpgsql(connectionString);
                break;
            case DatabaseType.Sqlite:
                optionsBuilder.UseSqlite(connectionString);
                break;
            default:
                throw new ArgumentException("不支援的資料庫類型", nameof(databaseType));
        }
        
        return optionsBuilder.Options;
    }
} 