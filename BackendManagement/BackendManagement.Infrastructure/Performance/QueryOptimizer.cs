using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BackendManagement.Infrastructure.Performance;

/// <summary>
/// 查詢優化器
/// </summary>
public static class QueryOptimizer
{
    /// <summary>
    /// 優化查詢
    /// </summary>
    public static IQueryable<T> Optimize<T>(this IQueryable<T> query)
        where T : EntityBase
    {
        // 加入 NoTracking
        if (query is IQueryable<EntityBase>)
        {
            query = query.AsNoTracking();
        }

        // 加入索引提示
        if (query.Provider is EntityQueryProvider)
        {
            query = query.TagWith("WITH (INDEX(IX_Id))");
        }

        return query;
    }

    /// <summary>
    /// 分頁查詢
    /// </summary>
    public static async Task<(List<T> Items, int Total)> PaginateAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    /// <summary>
    /// 批次查詢
    /// </summary>
    public static async Task<List<T>> BatchAsync<T>(
        this IQueryable<T> query,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
    {
        var result = new List<T>();
        var total = await query.CountAsync(cancellationToken);
        var batches = (int)Math.Ceiling(total / (double)batchSize);

        for (var i = 0; i < batches; i++)
        {
            var batch = await query
                .Skip(i * batchSize)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            result.AddRange(batch);
        }

        return result;
    }
} 