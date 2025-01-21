namespace BackendManagement.Domain.Exceptions;

/// <summary>
/// 實體未找到例外
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"找不到 {entityName} (ID: {id})")
    {
        EntityName = entityName;
        Id = id;
    }

    public string EntityName { get; }
    public object Id { get; }
} 