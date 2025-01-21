namespace BackendManagement.Domain.Interfaces;

/// <summary>
/// 儲存庫介面
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// 取得實體
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得所有實體
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根據條件取得實體
    /// </summary>
    Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增實體
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次新增實體
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新實體
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次更新實體
    /// </summary>
    Task UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除實體
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批次刪除實體
    /// </summary>
    Task DeleteRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    IQueryable<TEntity> AsQueryable();
} 