namespace BackendManagement.Infrastructure.Performance;

/// <summary>
/// 快取鍵值產生器
/// </summary>
public static class CacheKeyGenerator
{
    /// <summary>
    /// 產生實體快取鍵值
    /// </summary>
    public static string GenerateEntityCacheKey<TEntity>(Guid id)
        where TEntity : EntityBase
    {
        return $"Entity:{typeof(TEntity).Name}:{id}";
    }

    /// <summary>
    /// 產生查詢快取鍵值
    /// </summary>
    public static string GenerateQueryCacheKey<TEntity>(string queryName, params object[] parameters)
        where TEntity : EntityBase
    {
        var parameterString = string.Join(":", parameters);
        return $"Query:{typeof(TEntity).Name}:{queryName}:{parameterString}";
    }
} 