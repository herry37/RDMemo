using BackendManagement.Domain.Common;
using BackendManagement.Domain.Interfaces;
using BackendManagement.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendManagement.Infrastructure.Performance;

/// <summary>
/// 快取儲存庫裝飾器
/// </summary>
public class CachedRepository<TEntity> : Repository<TEntity> where TEntity : EntityBase
{
    private readonly ICacheService _cacheService;
    private readonly string _cachePrefix;

    public CachedRepository(
        ApplicationDbContext context,
        ICacheService cacheService) : base(context)
    {
        _cacheService = cacheService;
        _cachePrefix = typeof(TEntity).Name;
    }

    public override async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{_cachePrefix}:{id}";
        var cached = await _cacheService.GetAsync<TEntity>(cacheKey);
        
        if (cached != null)
            return cached;

        var entity = await base.GetByIdAsync(id, cancellationToken);
        if (entity != null)
            await _cacheService.SetAsync(cacheKey, entity);

        return entity;
    }

    public override async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{_cachePrefix}:all";
        var cached = await _cacheService.GetAsync<IEnumerable<TEntity>>(cacheKey);
        
        if (cached != null)
            return cached;

        var entities = await base.GetAllAsync(cancellationToken);
        await _cacheService.SetAsync(cacheKey, entities);
        return entities;
    }

    public override async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        // 對於條件查詢，不使用快取
        return await base.GetAsync(predicate, cancellationToken);
    }

    public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await base.AddAsync(entity, cancellationToken);
        await _cacheService.SetAsync($"{_cachePrefix}:{entity.Id}", result);
        await _cacheService.RemoveAsync($"{_cachePrefix}:all");
        return result;
    }

    public override async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);
        await _cacheService.SetAsync($"{_cachePrefix}:{entity.Id}", entity);
        await _cacheService.RemoveAsync($"{_cachePrefix}:all");
    }

    public override async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await base.DeleteAsync(entity, cancellationToken);
        await _cacheService.RemoveAsync($"{_cachePrefix}:{entity.Id}");
        await _cacheService.RemoveAsync($"{_cachePrefix}:all");
    }

    public override IQueryable<TEntity> AsQueryable()
    {
        return base.AsQueryable();
    }
} 