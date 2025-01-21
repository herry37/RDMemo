using BackendManagement.Domain.Common;
using BackendManagement.Domain.Interfaces;
using System.Linq.Expressions;

namespace BackendManagement.Infrastructure.Persistence;

/// <summary>
/// 儲存庫基礎類別
/// </summary>
public class RepositoryBase<TEntity> : IRepository<TEntity> 
    where TEntity : EntityBase
{
    private readonly IRepository<TEntity> _repository;

    protected RepositoryBase(IRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _repository.AddRangeAsync(entities, cancellationToken);
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _repository.UpdateRangeAsync(entities, cancellationToken);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(entity, cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteRangeAsync(entities, cancellationToken);
    }

    public virtual IQueryable<TEntity> AsQueryable()
    {
        return _repository.AsQueryable();
    }
} 